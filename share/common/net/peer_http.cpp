#include "stdafx.h"
#include "peer_http.h"
#include <boost/bind.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/algorithm/string.hpp>

using namespace boost;
using namespace boost::asio::ip;

peer_http::peer_http(boost::asio::io_service& io_service)
	: resolver_(io_service),
	socket_(io_service)
{
}

peer_http::~peer_http()
{
	if(socket_.is_open())
	{
		boost::system::error_code ec;
		socket_.close(ec);
	}	
}

void peer_http::post_request(const msg_request& req)
{
	std::ostream request_stream(&request_);
	if (req.method == "GET")
	{
		request_stream << req.method <<" "<< req.spath << " HTTP/"<< req.http_version_major<<"."<<req.http_version_minor<<"\r\n";
		request_stream << "Host: " << req.uri << "\r\n\r\n";
	}		
	else if (req.method == "POST")
	{
		request_stream << req.method <<" "<< req.spath << " HTTP/"<< req.http_version_major<<"."<<req.http_version_minor<<"\r\n";
		request_stream << "Host: " << req.uri << "\r\n";
		request_stream << req.headers[1].name << ": "<<req.headers[1].value <<"\r\n";
		request_stream << req.headers[0].name << ": "<<req.headers[0].value <<"\r\n\r\n";
		request_stream << req.content << "\r\n\r\n";
	}

	request_stream << "Accept: */*\r\n";
	//request_stream << "Pragma: no-cache\r\n";
	//request_stream << "Cache-Control: no-cache\r\n";
	request_stream << "Connection: close\r\n";

	// Start an asynchronous resolve to translate the server and service names
	// into a list of endpoints.
	std::string szService ("http");
	std::string szIp = req.uri;
	int i = req.uri.find(":") ;
	if (i != -1)
	{
		szService = req.uri.substr(i+1);
		szIp = req.uri.substr(0, i);
	}

	m_response.clear();
				
	// Get a list of endpoints corresponding to the server name.
	tcp::resolver::query query(szIp, szService);
	resolver_.async_resolve(query,
		boost::bind(&peer_http::handle_resolve, shared_from_this(),
		boost::asio::placeholders::error,
		boost::asio::placeholders::iterator));
}

void peer_http::handle_resolve(const boost::system::error_code& err,
					tcp::resolver::iterator endpoint_iterator)
{
	if (!err)
	{
		// Attempt a connection to each endpoint in the list until we
		// successfully establish a connection.
		boost::asio::async_connect(socket_, endpoint_iterator,
			boost::bind(&peer_http::handle_connect, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else
	{
		http_response(false, err.message());		
	}
}

void peer_http::handle_connect(const boost::system::error_code& err)
{
	if (!err)
	{
		// The connection was successful. Send the request.
		boost::asio::async_write(socket_, request_,
			boost::bind(&peer_http::handle_write_request, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else
	{
		http_response(false, err.message());	
	}
}

void peer_http::handle_write_request(const boost::system::error_code& err)
{
	if (!err)
	{
		// Read the response status line. The response_ streambuf will
		// automatically grow to accommodate the entire line. The growth may be
		// limited by passing a maximum size to the streambuf constructor.
		boost::asio::async_read_until(socket_, response_, "\r\n",
			boost::bind(&peer_http::handle_read_status_line, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else
	{
		http_response(false, err.message());	
	}
}

void peer_http::handle_read_status_line(const boost::system::error_code& err)
{
	if (!err)
	{
		// Check that response is OK.
		std::istream response_stream(&response_);
		response_stream >> m_response.http_version;
		response_stream >> m_response.status_code;
		std::getline(response_stream, m_response.status_message);
		boost::trim(m_response.status_message);
		if (!response_stream || m_response.http_version.substr(0, 5) != "HTTP/")
		{
			http_response(false, "Invalid response\n");
			return;
		}
		if (m_response.status_code != 200)
		{
			std::string ret = "Response returned with status code " + lexical_cast<std::string>(m_response.status_code) ;
			http_response(false, ret);			
			return;
		}

		// Read the response headers, which are terminated by a blank line.
		boost::asio::async_read_until(socket_, response_, "\r\n\r\n",
			boost::bind(&peer_http::handle_read_headers, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else
	{
		http_response(false, err.message());	
	}
}

void peer_http::handle_read_headers(const boost::system::error_code& err)
{
	if (!err)
	{
		// Process the response headers.
		std::istream response_stream(&response_);

		std::string strLine;  
		while (std::getline(response_stream, strLine) && strLine != "\r")
		{  
			header h;
			h.name = strLine.substr(0, strLine.find_first_of(":"));
			h.value = strLine.substr(h.name.size()+1, strLine.find_first_of("\r"));
			boost::trim(h.value);
			m_response.headers.push_back(h);
		}  

		// Write whatever content we already have to output.
		if (response_.size() > 0)
		{
			m_response.content << &response_;
		}

		// Start reading remaining data until EOF.
		boost::asio::async_read(socket_, response_,
			boost::asio::transfer_at_least(1),
			boost::bind(&peer_http::handle_read_content, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else
	{
		http_response(false, err.message());	
	}
}

void peer_http::handle_read_content(const boost::system::error_code& err)
{
	if (!err)
	{
		// Write all of the data that has been read so far.
		m_response.content << &response_;

		// Continue reading remaining data until EOF.
		boost::asio::async_read(socket_, response_,
			boost::asio::transfer_at_least(1),
			boost::bind(&peer_http::handle_read_content, shared_from_this(),
			boost::asio::placeholders::error));
	}
	else if (err != boost::asio::error::eof)
	{
		http_response(false, err.message());		
	}
	else
	{		
		http_response(true, m_response.content.str());		
	}
}

#include <net/peer_http_mgr.h>
void peer_http::use_synchronization()
{
	peer_http_mgr::instance().postTask(shared_from_this());
}