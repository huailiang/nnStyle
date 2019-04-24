/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/


#ifndef __std__
#define __std__


/*
used in encoder & decoder
*/

#define CACHE_MAX 2048
#define CACHE_HALF 1024

groupshared float g_cache[CACHE_MAX];


#define StdIndex(x, y, z, width, depth) \
	((width) * (depth) * (x) + (depth) * (y) + z)


#define StdID(id, width, depth)	\
	((width) * (depth) * id.x + (depth) * id.y + id.z)


#define StdPad(id, width, depth, pad)	\
	uint low = pad - id.x;	\
	uint mid = id.x - pad;	\
	uint high = 2 * width + pad - id.x;	\
	uint x_array[3] = { low, mid, high };	\
	low = pad - id.y;	\
	mid = id.y - pad;	\
	high = 2 * width + pad - id.y;	\
	uint y_array[3] = { low, mid, high };	\
	uint x_id = id.x > (pad + width) ? 2 : saturate(id.x / pad);	\
	uint y_id = id.y > (pad + width) ? 2 : saturate(id.y / pad);	\
	x_id = x_array[x_id];	\
	y_id = y_array[y_id];	\
	uint indx = width * depth * x_id + depth * y_id + id.z;	\
	uint indx2 = StdID(id, width + pad * 2, depth);	\


void StdSeq(int x, int y, int z, int width,int depth, out int result[9])
{
	result[0] = StdIndex(x,	y,	 z,	width,	depth);
	result[1] = StdIndex(x+1,	y,	 z,	width,	depth);
	result[2] = StdIndex(x+2,	y,	 z,	width,	depth);
	result[3] = StdIndex(x,	y+1, z,	width,	depth);
	result[4] = StdIndex(x+1,	y+1, z,	width,	depth);
	result[5] = StdIndex(x+2,	y+1, z,	width,	depth);
	result[6] = StdIndex(x,	y+2, z,	width,	depth);
	result[7] = StdIndex(x+1,	y+2, z,	width,	depth);
	result[8] = StdIndex(x+2,  y+2, z,	width,	depth);
}


inline bool StdCheckRange(uint3 id, uint width)
{
	return id.x >= width || id.y >= width;
}


#endif