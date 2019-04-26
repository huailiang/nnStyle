/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __decoder__
#define __decoder__

#include "libConst.cginc"
#include "libStd.cginc"
#include "libActive.cginc"

#define DefineDecodeBuffer(seq)	\
	RWStructuredBuffer<float> decoder_conv##seq##;	\
	RWStructuredBuffer<float> decoder_conv##seq##_conved;		\
	RWStructuredBuffer<float> decoder_conv##seq##_statistic;	\


#define DefineResiduleConv(id, input, output, seq, idx) \
	for(int j = 0;j < depth; j++) \
	{ 	\
		float v = 0.0f;	\
		for(int i= 0; i < depth; i++)	\
		{	\
			float3x3 xsamp = StdSample(decoder_conv0, id.x, id.y, i, input, depth);	\
			float3x3 kernel = decoder_g_r##seq##_c##idx##_Conv_weights[depth * i + j];	\
			float3x3 conv = xsamp * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (output * depth) * id.x + depth * id.y + j;	\
		input_writable[indx] = v;	\
	}


#define DeifineResiduleInst(id, seq, r)	\
	int indx = StdID(id, width, depth);	\
	float color = input_writable[indx];	\
	float mean = decoder_conv0_statistic[id.z * 2];	\
	float variance = decoder_conv0_statistic[id.z * 2 + 1];	\
	float inv = rsqrt(variance + EPSILON);	\
	float normalized = (color - mean) * inv;	\
	float scale = decoder_g_r##seq##_bn##r##_scale[id.z];	\
	float offset = decoder_g_r##seq##_bn##r##_offset[id.z];	\
	input_writable[indx] = scale * normalized + offset;		


#define DefineDecoderConv(id, width, depth1, depth2, idx)	\
	for(uint j = 0; j < depth2; j++) \
	{ 	\
		float v = 0.0f;	\
		for(uint i = 0; i < depth1; i++)	\
		{	\
			float3x3 xsamp = StdSlowSample(decoder_conv##idx##_conved, id.x, id.y, i, width, depth1);	\
			float3x3 kernel = decoder_g_d##idx##_dc_conv2d_Conv_weights[depth2 * i + j];	\
			float3x3 conv = xsamp * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (width * depth2) * id.x + depth2 * id.y + j;	\
		decoder_conv##idx##[indx] = v;	\
	}


#define DefineDecoderInstRelu(id, seq)	\
	int inx = StdID(id, width, depth);	\
	float color = decoder_conv##seq##[inx];	\
	float mean = decoder_conv##seq##_statistic[id.z * 2];	\
	float variance = decoder_conv##seq##_statistic[id.z * 2 + 1];	\
	float inv = rsqrt(variance + EPSILON);	\
	float normalized = (color - mean) * inv;	\
	float scale = decoder_g_d##seq##_bn_scale[id.z];	\
	float offset = decoder_g_d##seq##_bn_offset[id.z];	\
	decoder_conv##seq##[inx] = relu(scale * normalized + offset);


#define DefineDecoderPad(id, idx)	\
	int ninx1 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y) + id.z;	\
	int ninx2 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y + 1) + id.z;	\
	int ninx3 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y) + id.z;	\
	int ninx4 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y + 1) + id.z;	\
	decoder_conv##idx##_conved[ninx1] = v;	\
	decoder_conv##idx##_conved[ninx2] = v;	\
	decoder_conv##idx##_conved[ninx3] = v;	\
	decoder_conv##idx##_conved[ninx4] = v;	\


#define DefineDecoderExpand(id, idx, pidx) \
	int indx = StdID(id, width, depth);	\
	float v = decoder_conv##pidx##[indx];	\
	DefineDecoderPad(id, idx)


#endif