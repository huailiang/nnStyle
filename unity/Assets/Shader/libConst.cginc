/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __const__
#define __const__


#define e  		2.7182818
#define HALF_MAX        65504.0 // (2 - 2^-10) * 2^15
#define HALF_MAX_MINUS1 65472.0 // (2 - 2^-9) * 2^15
#define EPSILON         1.0e-5
#define PI              3.14159265359
#define TWO_PI          6.28318530718
#define FOUR_PI         12.56637061436
#define INV_PI          0.31830988618
#define INV_TWO_PI      0.15915494309
#define INV_FOUR_PI     0.07957747155
#define HALF_PI         1.57079632679
#define INV_HALF_PI     0.636619772367

//compute shader features
#define MAX_THREAD_Z	64

#ifdef SHADER_API_METAL  //ios or mac 
#define MAX_THREAD		512
#define MAX_THREAD_X	512
#define MAX_THREAD_Y	512
#define REV_THREAD_Z    8  // as MAX_THREAD/MAX_THREAD_Z
#define THREAD_Y_16Z    32
#define THREAD_Y_32Z    16 //MAX_THREAD/32
#define THREAD_Y_64Z    8
#define THREAD_Y_128Z   4
#define THREAD_Y_256Z   2
#else
#define MAX_THREAD		1024
#define MAX_THREAD_X	1024
#define MAX_THREAD_Y	1024
#define REV_THREAD_Z    16  // as 1024/64
#define THREAD_Y_16Z    64
#define THREAD_Y_32Z    32
#define THREAD_Y_64Z    16
#define THREAD_Y_128Z   8
#define THREAD_Y_256Z   4
#endif

#define MAX_GROUP_SHARED	8192 //globalshared's count max is 8192's float (equal 32768bytes)


#define FLT_EPSILON     1.192092896e-07 // Smallest positive number, such that 1.0 + FLT_EPSILON != 1.0
#define FLT_MIN         1.175494351e-38 // Minimum representable positive floating-point number
#define FLT_MAX         3.402823466e+38 // Maximum representable floating-point number


#endif