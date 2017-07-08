#pragma once
#include <enable_smart_ptr.h>

#ifdef WIN32
#ifdef _DEBUG
#pragma comment(lib, "libprotobuf-gd.lib")
#else
#pragma comment(lib, "libprotobuf.lib")
#endif

#endif



#include <enable_object_factory.h>
#include <enable_singleton.h>
#include <com_log.h>
//#include <net/peer_tcp.h>


template <class P, typename T, typename P2 = void>
class packet_factory :
	public object_factory_handler<T>
{
public:
	packet_factory(){}
	virtual ~packet_factory(){}

	virtual bool packet_process(boost::shared_ptr<void> peer, boost::shared_ptr<void> msg)
	{		
		auto tp = CONVERT_POINT(P, peer);
		print_info(tp);
		return packet_process(tp, CONVERT_POINT(T, msg));
	}

	//注册后的协议需要实现
	virtual bool packet_process(boost::shared_ptr<P> peer, boost::shared_ptr<T> msg) = 0;

	virtual bool packet_process(boost::shared_ptr<void> peer, uint32_t sessionid, boost::shared_ptr<void> msg)
	{		
		auto tp = CONVERT_POINT(P, peer);
		print_info(tp);
		return packet_process(tp, sessionid, CONVERT_POINT(T, msg));
	}

	//注册后的协议需要实现
	virtual bool packet_process(boost::shared_ptr<P> peer, uint32_t sessionid, boost::shared_ptr<T> msg) = 0;

	virtual bool packet_process(boost::shared_ptr<void> peer, boost::shared_ptr<void> player, boost::shared_ptr<void> msg) 
	{
		auto tp = CONVERT_POINT(P, peer);
		print_info(tp);
		return packet_process(tp,  CONVERT_POINT(P2, player), CONVERT_POINT(T, msg));
	};

	virtual bool packet_process(boost::shared_ptr<P> peer, boost::shared_ptr<P2> player, boost::shared_ptr<T> msg) = 0;

	virtual void print_info(boost::shared_ptr<P> peer){};
};


class packet_manager:
	public enable_object_factory,
	public enable_singleton<packet_manager>
{
public:
	packet_manager(){}
	virtual ~packet_manager(){}

};

#define PACKET_REGEDIT_RECV(ptype, packet) \
class packet##_factory : public packet_factory<ptype, packet>\
{\
public:\
	packet##_factory()\
	{\
\
	};\
	static void regedit_factory()\
	{\
		packet tmp;\
		packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
	};\
	virtual ~packet##_factory(){}\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg);\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<void> player, boost::shared_ptr<packet> msg){return false;};\
};\


#define PACKET_REGEDIT_SEND(packet) \
class packet##_factory : public packet_factory<peer_tcp, packet>\
{\
public:\
	packet##_factory()\
	{\
\
	}\
	static void regedit_factory()\
	{\
		packet tmp;\
		packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
	}\
	virtual ~packet##_factory(){}\
	virtual bool packet_process(boost::shared_ptr<peer_tcp> peer, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<peer_tcp> peer, uint32_t sessionid, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<peer_tcp> peer, boost::shared_ptr<void> player, boost::shared_ptr<packet> msg){return false;};\
};\

#ifdef DEBUG

#define PACKET_REGEDIT_RECV_LOG(ptype, packet) \
class packet##_factory : public packet_factory<ptype, packet>\
{\
public:\
	packet##_factory()\
{\
	\
};\
	static void regedit_factory()\
{\
	packet tmp;\
	packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
};\
	virtual ~packet##_factory(){}\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg);\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<void> player, boost::shared_ptr<packet> msg){return false;};\
	virtual void print_info(boost::shared_ptr<ptype> peer)\
	{\
		if(!peer) return;\
		SLOG_NOTICE << "packet_process id:"<<peer->get_id() << " patcket:" << typeid(packet).name();\
	}\
};\

#else 

#define PACKET_REGEDIT_RECV_LOG(ptype, packet) PACKET_REGEDIT_RECV(ptype, packet)
#endif

#define PACKET_REGEDIT_RECVGATE(ptype, packet, pplayer) \
class packet##_factory : public packet_factory<ptype, packet, pplayer>\
{\
public:\
	packet##_factory()\
{\
	\
};\
	static void regedit_factory()\
{\
	packet tmp;\
	packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
};\
	virtual ~packet##_factory(){}\
	virtual bool is_from_gate(){return true;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<pplayer> player, boost::shared_ptr<packet> msg);\
};\


#ifdef DEBUG

#define PACKET_REGEDIT_RECVGATE_LOG(ptype, packet,pplayer) \
class packet##_factory : public packet_factory<ptype, packet,pplayer>\
{\
public:\
	packet##_factory()\
{\
	\
};\
	static void regedit_factory()\
{\
	packet tmp;\
	packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
};\
	virtual ~packet##_factory(){}\
	virtual bool is_from_gate(){return true;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<pplayer> player, boost::shared_ptr<packet> msg);\
	virtual void print_info(boost::shared_ptr<ptype> peer)\
	{\
		if(!peer) return;\
		SLOG_NOTICE << "packet_process id:"<<peer->get_id() << " patcket:" << typeid(packet).name();\
	}\
};\

#else 

#define PACKET_REGEDIT_RECVGATE_LOG(ptype, packet,pplayer) PACKET_REGEDIT_RECVGATE(ptype, packet,pplayer)

#endif // DEBUG

#define PACKET_REGEDIT_RECVGATE_SID(ptype, packet) \
class packet##_factory : public packet_factory<ptype, packet>\
{\
	public:\
	packet##_factory()\
	{\
	\
	};\
	static void regedit_factory()\
	{\
	packet tmp;\
	packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
	};\
	virtual ~packet##_factory(){}\
	virtual bool is_from_gate(){return true;};\
	virtual bool use_sessionid(){return true;}\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg);\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<void> player, boost::shared_ptr<packet> msg){return false;};\
};\

#ifdef DEBUG

#define PACKET_REGEDIT_RECVGATE_SID_LOG(ptype, packet) \
class packet##_factory : public packet_factory<ptype, packet>\
{\
	public:\
	packet##_factory()\
	{\
	\
	};\
	static void regedit_factory()\
	{\
	packet tmp;\
	packet_manager::instance().regedit_object(tmp.packet_id(), boost::make_shared<packet##_factory>());\
	};\
	virtual ~packet##_factory(){}\
	virtual bool is_from_gate(){return true;};\
	virtual bool use_sessionid(){return true;}\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<packet> msg){return false;};\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, uint32_t sessionid, boost::shared_ptr<packet> msg);\
	virtual bool packet_process(boost::shared_ptr<ptype> peer, boost::shared_ptr<void> player, boost::shared_ptr<packet> msg){return false;};\
	virtual void print_info(boost::shared_ptr<ptype> peer)\
	{\
		if(!peer) return;\
		SLOG_NOTICE << "packet_process id:"<<peer->get_id() << " patcket:" << typeid(packet).name();\
	}\
};\

#else 

#define PACKET_REGEDIT_RECVGATE_SID_LOG(ptype, packet) PACKET_REGEDIT_RECVGATE_SID(ptype, packet)

#endif // DEBUG


#define PACKET_CREATE(packet_type, packet_id) CONVERT_POINT(packet_type, packet_manager::instance().create(packet_id))










