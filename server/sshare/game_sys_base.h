#pragma  once
#include <enable_smart_ptr.h>

class game_sys_handler
{

};

class game_sys_base
{
public:
	typedef boost::shared_ptr<game_sys_base> SysBasePtr;
	game_sys_base()
	{
		m_handler = nullptr;
	};
	virtual ~game_sys_base()
	{
		m_handler = nullptr;
	};
	void attach(game_sys_handler* manager)
	{
		m_handler = manager;
		on_attach();
	}

	virtual void on_attach(){};
	//注册数据
	virtual void init_sys_object(){};
	//加载数据
	virtual bool sys_load(){return true;};
	//初始化数据
	virtual void sys_init(){};
	// 每帧更新
	virtual void sys_update(double delta){}
	//0点更新
	virtual void sys_time_update(){};
	//系统类型game_sys_def.h
	virtual uint16_t get_sys_type()=0;
	
	// 系统退出
	virtual void sys_exit(){}

	template<class T>
	T* get_owner()
	{
		return static_cast<T*>(m_handler);
	};
private:
	game_sys_handler* m_handler;
};


//必须使用
#define MAKE_SYS_TYPE(stype)\
virtual uint16_t get_sys_type()\
{\
	return stype;\
};\
static uint16_t static_type()\
{\
	return stype;\
};\


//可选
#define MAKE_GET_OWNER(stype)\
stype* get_##stype()\
{\
	return get_owner<stype>();\
};\




