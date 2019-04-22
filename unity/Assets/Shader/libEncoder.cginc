/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __encoder__
#define __encoder__

#include "libConst.cginc"
#include "libActive.cginc"
#include "libStd.cginc"


#define DefineEncoderConv(id, input, output, depth1, depth2, stride, idx, pidx)	\
	for(uint j = 0; j < depth2; j++)	\
	{	\
		float v = 0.0f;	\
		for(uint i = 0; i < depth1; i++)	\
		{	\
			int seq[9];	\
			StdSeq(id.x*stride, id.y*stride, i, input, depth1, seq);	\
			float3x3 xsam = float3x3(encoder_conv##pidx##[seq[0]], encoder_conv##pidx##[seq[1]], encoder_conv##pidx##[seq[2]], \
				encoder_conv##pidx##[seq[3]], encoder_conv##pidx##[seq[4]], encoder_conv##pidx##[seq[5]], \
				encoder_conv##pidx##[seq[6]], encoder_conv##pidx##[seq[7]], encoder_conv##pidx##[seq[8]]); \
			float3x3 conv = encoder_g_e##idx##_c_Conv_weights[depth2 * i + j];	\
			float3x3 imul = xsam * conv;	\
			float3 iall = imul[0] + imul[1]+ imul[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (output * depth2) * id.x  + depth2 * id.y  + j;	\
		encoder_conv##idx##[indx] = v;	\
	}


#define InnerEncoderNormal(seq)	\
{	\
	for (uint i = 0; i < scale; i++)	\
	{	\
		int idx = i * nwidth * depth + (id.y * scale + i) *depth + id.z;	\
		g_cache[nix] += encoder_conv##seq##[idx];	\
		g_cache[nix + offset] += pow(abs(encoder_conv##seq##[idx]), 2);	\
	}	\
}


#define DefineEnNormal(id, width, depth, scale, seq)	\
	uint offset = width * depth;	\
	uint nix = id.y * depth + id.z;	\
	uint nwidth = width * scale;	\
	g_cache[nix] = 0;	\
	g_cache[nix + offset] = 0;	\
	for (uint i = 0; i < nwidth; i++)	\
	{	\
		InnerEncoderNormal(seq)	\
	}	\
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
		encoder_conv##seq##_statistic[id.z * 2] = mean;	\
		encoder_conv##seq##_statistic[id.z * 2 + 1] = qrt / len - pow(abs(mean), 2);	\
	}


#define DefineEnInstRelu(id, width, depth, seq)	\
	int inx = StdID(id, width, depth);	\
	float color = encoder_conv##seq##[inx];	\
	float mean = encoder_conv##seq##_statistic[id.z * 2];	\
	float variance = encoder_conv##seq##_statistic[id.z * 2 + 1];	\
	float inv = rsqrt(variance + EPSILON);	\
	float normalized = (color - mean) * inv;	\
	float scale = encoder_g_e##seq##_bn_scale[id.z];	\
	float offset = encoder_g_e##seq##_bn_offset[id.z];	\
	encoder_conv##seq##[inx] = relu(scale * normalized + offset);



#define DefineEnNormalMaxZ(id, width, depth, scale, seq)	\
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
			g_cache[nix] += encoder_conv##seq##[idx];	\
			g_cache[nix + offset] += pow(abs(encoder_conv##seq##[idx]), 2);	\
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
		encoder_conv##seq##_statistic[id.z * 2 + shift] = mean;	\
		encoder_conv##seq##_statistic[id.z * 2 + 1 + shift] = qrt / len - pow(abs(mean), 2);	\
	}

#endif