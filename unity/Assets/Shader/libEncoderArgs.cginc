/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __encoder_arg__
#define __encoder_arg__

//network args 
StructuredBuffer<float3x3> encoder_g_e1_c_Conv_weights;
StructuredBuffer<float3x3> encoder_g_e2_c_Conv_weights;
StructuredBuffer<float3x3> encoder_g_e3_c_Conv_weights;
StructuredBuffer<float3x3> encoder_g_e4_c_Conv_weights;
StructuredBuffer<float3x3> encoder_g_e5_c_Conv_weights;

StructuredBuffer<float> encoder_g_e0_bn_offset;
StructuredBuffer<float> encoder_g_e0_bn_scale;
StructuredBuffer<float> encoder_g_e1_bn_offset;
StructuredBuffer<float> encoder_g_e1_bn_scale;
StructuredBuffer<float> encoder_g_e2_bn_offset;
StructuredBuffer<float> encoder_g_e2_bn_scale;
StructuredBuffer<float> encoder_g_e3_bn_offset;
StructuredBuffer<float> encoder_g_e3_bn_scale;
StructuredBuffer<float> encoder_g_e4_bn_offset;
StructuredBuffer<float> encoder_g_e4_bn_scale;
StructuredBuffer<float> encoder_g_e5_bn_offset;
StructuredBuffer<float> encoder_g_e5_bn_scale;


#endif