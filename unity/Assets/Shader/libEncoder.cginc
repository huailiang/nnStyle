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
#include "libStd.cginc"

#define DefineEncoderConv(id, input, output, depth1, depth2, stride, idx, pidx)	\
	for(uint j = 0; j < depth2; j++)	\
	{	\
		float v = 0.0f;	\
		for(uint i = 0; i < depth1; i++)	\
		{	\
			int seq[9];	\
			StdOrderSeq(id.x*stride, id.y*stride, i, input, depth1, seq);	\
			float3x3 xsam = float3x3(encoder_conv##pidx##[seq[0]], encoder_conv##pidx##[seq[1]], encoder_conv##pidx##[seq[2]], \
									encoder_conv##pidx##[seq[3]], encoder_conv##pidx##[seq[4]], encoder_conv##pidx##[seq[5]], \
									encoder_conv##pidx##[seq[6]], encoder_conv##pidx##[seq[7]], encoder_conv##pidx##[seq[8]]); \
			float3x3 conv = encoder_g_e##idx##_c_Conv_weights[output*i+j];	\
			float3x3 imul = xsam * conv;	\
			float3 iall = imul[0] + imul[1]+ imul[2];	\
			v += iall[0] + iall[1] + iall[2];	\
		}	\
		int indx = (output * depth2) * id.x + depth2 * id.y + j;	\
		encoder_conv##idx##[indx] = v;	\
	}


#define DefineEncoderNormal(i, depth, idx)	\
	float mean = encoder_conv##idx##_statistic[i]/depth;	\
	float variance = encoder_conv##idx##_statistic[i+1]/depth - pow(mean,2);	\
	encoder_conv##idx##_statistic[i] =  mean;	\
	encoder_conv##idx##_statistic[i+1] = variance;


#define DefineInstanceRelu(id, width, depth, idx)	\
		float mean = encoder_conv##idx##_statistic[id.z*2];	\
		float variance = encoder_conv##idx##_statistic[id.z*2+1];	\
		float inv  = rsqrt(variance+EPSILON);	\
		int indx = StdOrderID(id, width, depth); 	\
		float normalize = (encoder_conv##idx##[indx] - mean) * inv;	\
		float scale = encoder_g_e##idx##_bn_scale[id.z];	\
		float offset = encoder_g_e##idx##_bn_offset[id.z];	\
		float rest = scale*normalize+offset;	\
		encoder_conv##idx##[indx] = relu(rest);	\
		

#endif