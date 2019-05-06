/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __encoder__
#define __encoder__


#include "libActive.cginc"
#include "libStd.cginc"


#define DefineEncodeBuffer(seq)	\
	RWStructuredBuffer<half> encoder_conv##seq##;	\
	RWStructuredBuffer<half> encoder_conv##seq##_statistic;	\


#define DefineEncoderArg(seq)	\
	StructuredBuffer<half> encoder_g_e##seq##_bn_offset;	\
	StructuredBuffer<half> encoder_g_e##seq##_bn_scale;	\
	StructuredBuffer<half3x3> encoder_g_e##seq##_c_Conv_weights;	\


#define DefineEncoderConv(id, input, output, depth1, depth2, stride, idx, pidx)	\
	for(uint j = 0; j < depth2; j++)	\
	{	\
		half v = 0.0f;	\
		for(uint i = 0; i < depth1; i++)	\
		{	\
			half3x3 xsam = StdSample(encoder_conv##pidx##, id.x*stride, id.y*stride, i, input, depth1);	\
			half3x3 conv = encoder_g_e##idx##_c_Conv_weights[depth2 * i + j];	\
			half3x3 imul = xsam * conv;	\
			half3 iall = imul[0] + imul[1]+ imul[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (output * depth2) * id.x  + depth2 * id.y  + j;	\
		encoder_conv##idx##[indx] = v;	\
	}


#define DefineEnInstRelu(id, width, depth, seq)	\
	int inx = StdID(id, width, depth);	\
	half color = encoder_conv##seq##[inx];	\
	half mean = encoder_conv##seq##_statistic[id.z * 2];	\
	half variance = encoder_conv##seq##_statistic[id.z * 2 + 1];	\
	half inv = rsqrt(variance + EPSILON);	\
	half normalized = (color - mean) * inv;	\
	half scale = encoder_g_e##seq##_bn_scale[id.z];	\
	half offset = encoder_g_e##seq##_bn_offset[id.z];	\
	encoder_conv##seq##[inx] = relu(scale * normalized + offset);


#endif