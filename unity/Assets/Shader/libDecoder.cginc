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

#define DefineResidulePad(id) \
	int low =   width - id.x - 1;	\
	int mid =  id.x - pad;	\
	int high = 2 * pad + width - 1 - id.x;	\
	int x_array[3] = { low, mid, high };	\
	low =  width - id.y - 1;	\
	mid =  id.y - pad;	\
	high = 2 * pad + width - 1 - id.y;	\
	int y_array[3] = { low, mid, high };	\
	int x_id = id.x > (pad + width) ? 2 : saturate(id.x / pad);	\
	int y_id = id.y > (pad + width) ? 2 : saturate(id.y / pad);	\
	x_id = x_array[x_id];	\
	y_id = y_array[y_id];	\
	int indx = StdIndex(x_id, y_id, id.z, width, depth); \
	int indx2 = StdID(id, (width+pad*2), depth);	


#define DefineResiduleConv(id, r, idx) \
	for(int j = 0;j < depth; j++) \
	{ 	\
		float v = 0.0f;	\
		for(int i= 0; i < depth; i++)	\
		{	\
			int seq[9]; \
			StdSeq(id.x, id.y, i, width, depth, seq);	\
			float3x3 sample = float3x3(decoder_residule[seq[0]], decoder_residule[seq[1]],decoder_residule[seq[2]],	\
									decoder_residule[seq[3]],decoder_residule[seq[4]],decoder_residule[seq[5]],	\
									decoder_residule[seq[6]],decoder_residule[seq[7]],decoder_residule[seq[8]]);	\
			float3x3 kernel = decoder_g_r##r##_c##idx##_Conv_weights[depth * i + j];	\
			float3x3 conv = sample * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (width * depth) * id.x + depth * id.y + j;	\
		input_writable[indx] = v;	\
	}


#define DefineResiduleNormal(id) \
	uint count = depth / MAX_THREAD_Z;	\
	uint offset = width * depth / count;	\
	for (uint i = 0; i < count; i++) {	\
		uint shift = offset * i * 2;	\
		uint z = id.z + MAX_THREAD_Z * i;	\
		uint nix = id.y * depth / count + id.z + shift;	\
		g_cache[nix] = 0;	\
		g_cache[nix + offset] = 0;	\
		for (uint j = 0; j < width * scale; j++) { \
			int idx = j * width * depth * scale + id.y * depth * scale + z;		\
			g_cache[nix] += input_writable[idx];	\
			g_cache[nix + offset] += pow(abs(input_writable[idx]), 2);	\
		}	\
	} 	\
	GroupMemoryBarrierWithGroupSync();	\
	if (id.y < count) {	\
		float mean = 0, qrt = 0;	\
		int shift = width * depth * id.y * 2 / count;	\
		for (uint i = 0; i < width; i++) {	\
			int idx = i * depth / count + id.z + shift;	\
			mean += g_cache[idx];	\
			qrt += g_cache[idx + offset];	\
		}	\
		int len = width * width * scale;	\
		mean = mean / len;	\
		input_statistic[id.z * 2 + shift] = mean;	\
		input_statistic[id.z * 2 + 1 + shift] = qrt / len - pow(abs(mean), 2);	\
	}


#define DeifineResiduleInst(id, seq, r)	\
	int indx = StdID(id, width, depth);	\
	float color = input_writable[indx];	\
	float mean = input_statistic[id.z * 2];	\
	float variance = input_statistic[id.z * 2 + 1];	\
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
			int seq[9];	\
			StdSeq(id.x, id.y, i, width, depth1, seq);	\
			float3x3 sample = float3x3(decoder_conv##idx##_conved[seq[0]], decoder_conv##idx##_conved[seq[1]], decoder_conv##idx##_conved[seq[2]], \
									decoder_conv##idx##_conved[seq[3]], decoder_conv##idx##_conved[seq[4]], decoder_conv##idx##_conved[seq[5]], \
									decoder_conv##idx##_conved[seq[6]], decoder_conv##idx##_conved[seq[7]], decoder_conv##idx##_conved[seq[8]]); \
			float3x3 kernel = decoder_g_d##idx##_dc_conv2d_Conv_weights[depth2 * i + j];	\
			float3x3 conv = sample * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (width * depth2) * id.x + depth2 * id.y + j;	\
		decoder_conv##idx##[indx] = v;	\
	}


#define InnerDecoderNormal(seq)	\
for (uint j = 0; j < nwidth; j++)	\
{	\
	[unroll]	\
	for (uint i = 0; i < intvl; i++)	\
	{	\
		int idx = j * nwidth * depth + (id.y + width * i) * depth + id.z;	\
		g_cache[nix] += decoder_conv##seq##[idx];	\
		g_cache[nix + offset] += pow(abs(decoder_conv##seq##[idx]), 2);	\
	}	\
}	\

#define DefineDecoderNormal(id, width, seq)	\
	uint offset = CACHE_HALF;	\
	uint nix = id.y * depth + id.z;	\
	uint scale = nwidth / width;	\
	uint intvl = id.y < nwidth % width ?  scale + 1 : scale;	\
	g_cache[nix] = 0;	\
	g_cache[nix + offset] = 0;	\
	InnerDecoderNormal(seq)	\
	GroupMemoryBarrierWithGroupSync();	\
	if (id.y == 0)	\
	{	\
		float mean = 0, qrt = 0;	\
		for (uint i = 0; i < width; i++)	\
		{	\
			int idx = i * depth + id.z;	\
			mean += g_cache[idx];	\
			qrt += g_cache[idx + offset];	\
		}	\
		int len = nwidth * nwidth;	\
		mean = mean / len;	\
		decoder_conv##seq##_statistic[id.z * 2] = mean;	\
		decoder_conv##seq##_statistic[id.z * 2 + 1] = qrt / len - mean * mean;	\
	}	\


#define DefineDecoderNormalMaxZ(id, width, depth, scale, seq)	\
	uint count = depth / MAX_THREAD_Z;	\
	uint offset = width * depth / count;	\
	for (uint i = 0; i < count; i++)	\
	{	\
		uint shift = offset * i * 2;	\
		uint z = id.z + MAX_THREAD_Z * i;	\
		uint nix = id.y * depth / count + id.z + shift;	\
		g_cache[nix] = 0;	\
		g_cache[nix + offset] = 0;	\
		for (uint j = 0; j < width * scale; j++)	\
		{	\
			int idx = j * width * depth * scale + id.y * depth * scale + z;	\
			g_cache[nix] += decoder_conv##seq##[idx];	\
			g_cache[nix + offset] += pow(abs(decoder_conv##seq##[idx]), 2);	\
		}	\
	}	\
	GroupMemoryBarrierWithGroupSync();	\
	if (id.y < count)	\
	{	\
		float mean = 0, qrt = 0;	\
		uint shift = width * depth * id.y * 2 / count;	\
		for (uint i = 0; i < width; i++)	\
		{	\
			int idx = i * depth / count + id.z + shift;	\
			mean += g_cache[idx];	\
			qrt += g_cache[idx + offset];	\
		}	\
		int len = width * width * scale;	\
		mean = mean / len;	\
		decoder_conv##seq##_statistic[id.z * 2 + shift] = mean;	\
		decoder_conv##seq##_statistic[id.z * 2 + 1 + shift] = qrt / len - pow(abs(mean), 2);	\
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


#define DefineDecoderExpand(id, width, depth, idx, pidx) \
	int indx = StdID(id, width, depth);	\
	float v = decoder_conv##pidx##[indx];	\
	int ninx1 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y) + id.z;	\
	int ninx2 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y + 1) + id.z;	\
	int ninx3 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y) + id.z;	\
	int ninx4 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y + 1) + id.z;	\
	decoder_conv##idx##_conved[ninx1] = v;	\
	decoder_conv##idx##_conved[ninx2] = v;	\
	decoder_conv##idx##_conved[ninx3] = v;	\
	decoder_conv##idx##_conved[ninx4] = v;

#endif