/*
Copyright (c) 2019 huailiang

# This file is part of neural network impleted with shader

contact: peng_huailiang@qq.com
*/


#ifndef __std__
#define __std__


#define StdOrderIndex(x, y, z, width, depth) \
	((width) * (depth) * (x) + (depth) * (y) + z)


#define StdOrderID(id, width, depth)	\
	(width * depth * id.x + depth * id.y + id.z)


void StdOrderSeq(int x, int y, int z, int width,int depth, out int result[9])
{
	result[0] = StdOrderIndex(x,	y,	 z,	width,	depth);
	result[1] = StdOrderIndex(x+1,	y,	 z,	width,	depth);
	result[2] = StdOrderIndex(x+2,	y,	 z,	width,	depth);
	result[3] = StdOrderIndex(x,	y+1, z,	width,	depth);
	result[4] = StdOrderIndex(x+1,	y+1, z,	width,	depth);
	result[5] = StdOrderIndex(x+2,	y+1, z,	width,	depth);
	result[6] = StdOrderIndex(x,	y+2, z,	width,	depth);
	result[7] = StdOrderIndex(x+1,	y+2, z,	width,	depth);
	result[8] = StdOrderIndex(x+2,  y+2, z,	width,	depth);
}



#endif