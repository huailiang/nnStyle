/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/


#ifndef __active__
#define __active__

#include "libConst.cginc"


float relu(float x)
{
	return max(x,0);
}

float lrelu(float x,float leak)
{
	return max(x, 0) + leak * min(x, 0);
}

float lrelu(float x)
{
	return lrelu(x,0.2);
}

float sigmod(float x)
{
	return 1/(1 + pow(e,-x));
}


#endif