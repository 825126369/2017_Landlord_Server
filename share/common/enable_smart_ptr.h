#pragma once
#include <boost/smart_ptr.hpp>

#ifndef CONVERT_POINT
#define CONVERT_POINT(dectype, srcptr) boost::static_pointer_cast<dectype>(srcptr)
#endif