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

#define CACHE_MAX  2048
#define CACHE_HALF 1024

groupshared float g_cache[CACHE_MAX];


#define StdIndex(x, y, z, width, depth) \
	((width) * (depth) * (x) + (depth) * (y) + z)


#define StdID(id, width, depth)	\
	((width) * (depth) * id.x + (depth) * id.y + id.z)


#define StdPad(id, width, depth, pad)	\
	uint low = pad - id.x;	\
	uint mid = id.x - pad;	\
	uint high = 2 * width + pad - 1 - id.x;	\
	uint x_array[3] = { low, mid, high };	\
	low = pad - id.y;	\
	mid = id.y - pad;	\
	high = 2 * width + pad - 1 - id.y;	\
	uint y_array[3] = { low, mid, high };	\
	uint x_id = id.x >= (pad + width) ? 2 : saturate(id.x / pad);	\
	uint y_id = id.y >= (pad + width) ? 2 : saturate(id.y / pad);	\
	x_id = x_array[x_id];	\
	y_id = y_array[y_id];	\
	uint indx = StdIndex(x_id, y_id, id.z, width, depth);	\
	uint indx2 = StdID(id, width + pad * 2, depth);	\


void StdSeq(int x, int y, int z, int width,int depth, out int res[9])
{
	res[0] = StdIndex(x,	y,	 z,	width,	depth);
	res[1] = StdIndex(x+1,	y,	 z,	width,	depth);
	res[2] = StdIndex(x+2,	y,	 z,	width,	depth);
	res[3] = StdIndex(x,	y+1, z,	width,	depth);
	res[4] = StdIndex(x+1,	y+1, z,	width,	depth);
	res[5] = StdIndex(x+2,	y+1, z,	width,	depth);
	res[6] = StdIndex(x,	y+2, z,	width,	depth);
	res[7] = StdIndex(x+1,	y+2, z,	width,	depth);
	res[8] = StdIndex(x+2,  y+2, z,	width,	depth);
}


/*
conv2d with valid padding
*/
float3x3 StdSample(RWStructuredBuffer<float> buffer, int x, int y, int z, int width, int depth)
{
	int sq[9];
	StdSeq(x, y, z, width, depth, sq);
	return float3x3(buffer[sq[0]], buffer[sq[1]], buffer[sq[2]],
		buffer[sq[3]], buffer[sq[4]], buffer[sq[5]],
		buffer[sq[6]], buffer[sq[7]], buffer[sq[8]]);
}

/*
conv2d with same padding 
xy range section will be filled with zero
*/
float3x3 StdSlowSample(RWStructuredBuffer<float> buffer, int x, int y, int z, int width, int depth)
{
	float a[9];
	bool x1 = x + 1 < width;
	bool x2 = x + 2 < width;
	bool y1 = y + 1 < width;
	bool y2 = y + 2 < width;
	a[0] = buffer[StdIndex(x, y, z, width, depth)];
	a[1] = x1 ? buffer[StdIndex(x + 1, y, z, width, depth)] : 0;
	a[2] = x2 ? buffer[StdIndex(x + 2, y, z, width, depth)] : 0;
	a[3] = y1 ? buffer[StdIndex(x, y + 1, z, width, depth)] : 0;
	a[4] = x1 && y1 ? buffer[StdIndex(x + 1, y + 1, z, width, depth)] : 0;
	a[5] = x2 && y1 ? buffer[StdIndex(x + 2, y + 1, z, width, depth)] : 0;
	a[6] = y2 ? buffer[StdIndex(x, y + 2, z, width, depth)] : 0;
	a[7] = x1 && y2 ? buffer[StdIndex(x + 1, y + 2, z, width, depth)] : 0;
	a[8] = x2 && y2 ? buffer[StdIndex(x + 2, y + 2, z, width, depth)] : 0;
	return float3x3(a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8]);
}


inline bool StdCheckRange(uint3 id, uint width)
{
	return id.x >= width || id.y >= width;
}

#define StdInnerNormal(inbuffer)	\
	[unroll]	\
	for (uint j = 0; j < nwidth; j++) {	\
		for (uint i = 0; i < intvl; i++) {	\
			int idx = j * nwidth * depth + (id.y + width * i) * depth + z;	\
			g_cache[nix] += inbuffer[idx];	\
			g_cache[nix + offset] += inbuffer[idx] * inbuffer[idx];	\
		}	\
	}


#define StdDefineNormal(id, inbuffer, outbuffer, width)	\
	uint offset = CACHE_HALF;	\
	uint z = id.x;	\
	uint nix = id.y * depth + z;	\
	uint scale = nwidth / width;	\
	uint intvl = id.y < nwidth % width ?  scale + 1 : scale;	\
	g_cache[nix] = 0;	\
	g_cache[nix + offset] = 0;	\
	StdInnerNormal(inbuffer)	\
	GroupMemoryBarrierWithGroupSync();	\
	if (id.y == 1)	\
	{	\
		float mean = 0, qrt = 0;	\
		for (uint i = 0; i < width; i++)	\
		{	\
			int idx = i * depth + z;	\
			mean += g_cache[idx];	\
			qrt += g_cache[idx + offset];	\
		}	\
		int len = nwidth * nwidth;	\
		mean = mean / len;	\
		outbuffer[z * 2] = mean;	\
		outbuffer[z * 2 + 1] = qrt / len - mean * mean;	\
	}	


#endif