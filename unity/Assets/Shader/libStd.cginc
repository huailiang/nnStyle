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
groupshared float g_cahce[2048];


#define StdIndex(x, y, z, width, depth) \
	((width) * (depth) * (x) + (depth) * (y) + z)


#define StdID(id, width, depth)	\
	(width * depth * id.x + depth * id.y + id.z)


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