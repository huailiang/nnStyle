/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/


#ifndef __active__
#define __active__

#include "libConst.cginc"


inline float relu(float x)
{
	return max(x,0);
}

inline half relu(half x)
{
	return max(x, 0);
}

inline float3 relu(float3 x)
{
	float x1 = relu(x.x);
	float x2 = relu(x.y);
	float x3 = relu(x.z);
	return float3(x1, x2, x3);
}

inline half3 relu(half3 x)
{
	return half3(relu(x.x), relu(x.y), relu(x.z));
}

inline float lrelu(float x,float leak)
{
	return max(x, 0) + leak * min(x, 0);
}

inline half lrelu(half x, half leak)
{
	return max(x, 0) + leak * min(x, 0);
}

inline float3 lrelu(float3 x, float leak)
{
	return float3(lrelu(x.x, leak), lrelu(x.y, leak), lrelu(x.z, leak));
}

inline half3 lrelu(half3 x, half leak)
{
	return half3(lrelu(x.x, leak), lrelu(x.y, leak), lrelu(x.z, leak));
}

inline float lrelu(float x)
{
	return lrelu(x,0.2);
}

inline half lrelu(half x)
{
	return lrelu(x, 0.2);
}

inline float3 lrelu(float3 x)
{
	return lrelu(x, 0.2);
}

inline half3 lrelu(half3 x)
{
	return lrelu(x, 0.2);
}

inline float sigmod(float x)
{
	return 1/(1 + pow(e,-x));
}

inline half sigmod(half x)
{
	return 1 / (1 + pow(e, -x));
}

inline float3 sigmod(float3 x)
{
	return float3(sigmod(x.x), sigmod(x.y), sigmod(x.z));
}

inline half3 sigmod(half3 x)
{
	return half3(sigmod(x.x), sigmod(x.y), sigmod(x.z));
}


#endif