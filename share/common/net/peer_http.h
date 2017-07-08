#pragma once

#include <string>
#include <vector>
#include <iostream>
#include <boost/asio.hpp>
#include <boost/enable_shared_from_this.hpp>

struct header
{
	std::string name;
	std::string value;
};

struct msg_request
{
public:
	explicit msg_request(){ clear();}
	
	std::string method;			// "GET", "POST".	
	std::string uri;			// 不包含http://的路径
	int http_version_major;		// 
	int http_version_minor;		// 
	std::vector<header> headers;// HTTP包头
	std::string content;		// 上传的数据
	std::string spath;			// 请求地址

	void clear()
	{
		method = "GET";
		uri.clear();
		http_version_major = 1;
		http_version_minor = 0;
		headers.clear();
		content.clear();
		spath.clear();

	}
};

struct msg_response
{
public:
	explicit msg_response(){ clear();}	
	std::string http_version;		// 版本
	int32_t status_code;			// 状态码
	std::string status_message;		// 状态
	std::vector<header> headers;	// HTTP包头
	std::stringstream content;		// HTTP返回的内容	

	void clear()
	{
		http_version.clear();
		status_code = -1;
		status_message.clear();
		headers.clear();
		content.str("");
	}
};

class peer_http: public boost::enable_shared_from_this<peer_http>
{
public:
	peer_http(boost::asio::io_service& io_service);
	virtual ~peer_http();

	void post_request(const msg_request& req);	
	//同步处理才使用
	virtual void on_complete(){};	
private:
	void handle_resolve(const boost::system::error_code& err, boost::asio::ip::tcp::resolver::iterator endpoint_iterator);
	void handle_connect(const boost::system::error_code& err);
	void handle_write_request(const boost::system::error_code& err);
	void handle_read_status_line(const boost::system::error_code& err);
	void handle_read_headers(const boost::system::error_code& err);
	void handle_read_content(const boost::system::error_code& err);

private:
	boost::asio::ip::tcp::resolver resolver_;
	boost::asio::ip::tcp::socket socket_;
	boost::asio::streambuf request_;
	boost::asio::streambuf response_;

protected:
	//http结果回调接口 (异步非线程安全)
	virtual void http_response(bool result, const std::string& response) = 0;

	//同步处理 使用peer_http_mgr从on_complete处理
	void use_synchronization();
	msg_response m_response;
};
