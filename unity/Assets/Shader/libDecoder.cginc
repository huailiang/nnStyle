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

#define DefineResidulePad(id, width, depth, pad) \
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
	int indx = StdOrderIndex(x_id, y_id, id.z, width, depth); \
	int indx2 = StdOrderID(id, (width+pad*2), depth);	\
	decoder_residule[indx2] = input_initial[indx];	


#define DefineResiduleConv(id, width, depth, r, idx) \
	for(int j = 0;j < depth; j++) \
	{ 	\
		float v = 0.0f;	\
		for(int i= 0; i < depth; i++)	\
		{	\
			int seq[9]; \
			StdOrderSeq(id.x, id.y, i, width, depth, seq);	\
			float3x3 sample = float3x3(decoder_residule[seq[0]], decoder_residule[seq[1]],decoder_residule[seq[2]],	\
									decoder_residule[seq[3]],decoder_residule[seq[4]],decoder_residule[seq[5]],	\
									decoder_residule[seq[6]],decoder_residule[seq[7]],decoder_residule[seq[8]]);	\
			float3x3 kernel = decoder_g_r##r##_c##idx##_Conv_weights[depth * j + i];	\
			float3x3 conv = sample * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (width * depth) * id.x + depth * id.y + j;	\
		input_writable[indx] = v;	\
	}


#define DefineResiduleDecNormal(id, width, depth, scale) \
	uint count = depth / MAX_THREAD_Z;	\
	uint offset = width * depth / count;	\
	for (uint i = 0; i < count; i++) {	\
		uint shift = offset * i * 2;	\
		uint z = id.z + MAX_THREAD_Z * i;	\
		uint nix = id.y * depth / count + id.z + shift;	\
		g_cahce[nix] = 0;	\
		g_cahce[nix + offset] = 0;	\
		for (uint j = 0; j < width * scale; j++) { \
			int idx = j * width * depth * scale + id.y * depth * scale + z;		\
			g_cahce[nix] += input_writable[idx];	\
			g_cahce[nix + offset] += pow(abs(input_writable[idx]), 2);	\
		}	\
	} 	\
	GroupMemoryBarrierWithGroupSync();	\
	if (id.y < count) {	\
		float mean = 0, qrt = 0;	\
		int shift = width * depth * id.y * 2 / count;	\
		for (uint i = 0; i < width; i++) {	\
			int idx = i * depth / count + id.z + shift;	\
			mean += g_cahce[idx];	\
			qrt += g_cahce[idx + offset];	\
		}	\
		int len = width * width * scale;	\
		mean = mean / len;	\
		input_statistic[id.z * 2 + shift] = mean;	\
		input_statistic[id.z * 2 + 1 + shift] = qrt / len - pow(abs(mean), 2);	\
	}


#define DefineResiduleNormal(id, width,depth,r,idx)	\
	float mean = input_statistic[id.z*2];	\
	float variance = input_statistic[id.z*2+1];	\
	float inv = rsqrt(variance + EPSILON);	\
	int indx = width * depth * id.x + depth * id.y + id.z;	\
	float normalize =  (decoder_residule[indx] - mean) * inv;	\
	float scale  = decoder_g_r##r##_bn##idx##_scale[id.z];	\
	float offset = decoder_g_r##r##_bn##idx##_offset[id.z];	\
	float rest = scale * normalize + offset;	\
	decoder_residule[indx] = rest;


#define DeifineResiduleInst(id, width, depth, seq, r)	\
	int indx = width * depth * id.x + depth * id.y + id.z;	\
	float color = input_writable[indx];	\
	float mean = input_statistic[id.z * 2];	\
	float variance = input_statistic[id.z * 2 + 1];	\
	float inv = rsqrt(variance + EPSILON);	\
	float normalized = (color - mean) * inv;	\
	float scale = decoder_g_r##seq##_bn##r##_scale[id.z];	\
	float offset = decoder_g_r##seq##_bn##r##_offset[id.z];	\
	input_initial[indx] = scale * normalized + offset;		
		

#define DefineDecoderConv(id, width, depth1, depth2, stride, idx)	\
	for(int j=0;j<depth2;j++) \
	{ 	\
		float v = 0.0f;	\
		for(int i=0;i<depth1;i++)	\
		{	\
			int seq[9];	\
			StdOrderSeq(id.x*stride, id.y*stride, i, width, depth1, seq);	\
			float3x3 sample = float3x3(decoder_conv##idx##[seq[0]], decoder_conv##idx##[seq[1]], decoder_conv##idx##[seq[2]], \
									decoder_conv##idx##[seq[3]], decoder_conv##idx##[seq[4]], decoder_conv##idx##[seq[5]], \
									decoder_conv##idx##[seq[6]], decoder_conv##idx##[seq[7]], decoder_conv##idx##[seq[8]]); \
			float3x3 kernel = decoder_g_d1_dc_conv2d_Conv_weights[depth1*j+i];	\
			float3x3 conv = sample * kernel;	\
			float3 iall = conv[0] + conv[1] + conv[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (width * depth2) * id.x + depth2 * id.y + j;	\
		decoder_conv##idx##_conved[indx] = v;	\
		decoder_conv##idx##_statistic[j*2] += v;		\
		decoder_conv##idx##_statistic[j*2+1] += pow(abs(v),2);	\
	}


#define DefineDecoderNormal(id, width, depth, idx)	\
	float mean = decoder_conv##idx##_statistic[id.z*2];	\
	float variance = decoder_conv##idx##_statistic[id.z*2+1];	\
	float inv = rsqrt(variance + EPSILON);	\
	int indx = width * depth * id.x + depth * id.y + id.z;	\
	float normalize =  (decoder_conv##idx##_conved[indx] - mean) * inv;	\
	float scale  = decoder_g_d##idx##_bn_scale[id.z];		\
	float offset = decoder_g_d##idx##_bn_offset[id.z];	\
	float rest = scale * normalize + offset;	\
	decoder_conv##idx##_conved[indx] = rest;


#define DefineDecoderExpand(id, width, depth, idx, pidx) \
	int indx = width * depth * id.x + depth * id.y + id.z;	\
	float v = decoder_conv##pidx##_conved[indx];	\
	int ninx1 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y) + id.z;	\
	int ninx2 = (2 * width) * depth * (2 * id.x) + depth * (2* id.y + 1) + id.z;	\
	int ninx3 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y) + id.z;	\
	int ninx4 = (2 * width) * depth * (2 * id.x+1) + depth * (2* id.y + 1) + id.z;	\
	decoder_conv##idx##[ninx1] = v;	\
	decoder_conv##idx##[ninx2] = v;	\
	decoder_conv##idx##[ninx3] = v;	\
	decoder_conv##idx##[ninx4] = v;

#endif