/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/

#ifndef __decoder_arg__
#define __decoder_arg__

//network args 
StructuredBuffer<float3x3> decoder_g_d1_dc_conv2d_Conv_weights;
StructuredBuffer<float3x3> decoder_g_d2_dc_conv2d_Conv_weights;
RWStructuredBuffer<float3x3> decoder_g_d3_dc_conv2d_Conv_weights;

StructuredBuffer<float3x3> decoder_g_r1_c1_Conv_weights;
StructuredBuffer<float3x3> decoder_g_r1_c2_Conv_weights;
StructuredBuffer<float3x3> decoder_g_r2_c1_Conv_weights;
StructuredBuffer<float3x3> decoder_g_r2_c2_Conv_weights;
StructuredBuffer<float3x3> decoder_g_r3_c1_Conv_weights;
StructuredBuffer<float3x3> decoder_g_r3_c2_Conv_weights;

StructuredBuffer<float> decoder_g_pred_c_Conv_weights; //7x7

StructuredBuffer<float> decoder_g_d1_bn_offset;
StructuredBuffer<float> decoder_g_d1_bn_scale;
StructuredBuffer<float> decoder_g_d2_bn_offset;
StructuredBuffer<float> decoder_g_d2_bn_scale;
StructuredBuffer<float> decoder_g_d3_bn_offset;
StructuredBuffer<float> decoder_g_d3_bn_scale;

StructuredBuffer<float> decoder_g_r1_bn1_offset;
StructuredBuffer<float> decoder_g_r1_bn1_scale;
StructuredBuffer<float> decoder_g_r1_bn2_offset;
StructuredBuffer<float> decoder_g_r1_bn2_scale;
StructuredBuffer<float> decoder_g_r2_bn1_offset;
StructuredBuffer<float> decoder_g_r2_bn1_scale;
StructuredBuffer<float> decoder_g_r2_bn2_offset;
StructuredBuffer<float> decoder_g_r2_bn2_scale;
StructuredBuffer<float> decoder_g_r3_bn1_offset;
StructuredBuffer<float> decoder_g_r3_bn1_scale;
StructuredBuffer<float> decoder_g_r3_bn2_offset;
StructuredBuffer<float> decoder_g_r3_bn2_scale;

#endif