# encoding=utf8

import os, shutil
import tensorflow as tf
import struct
from tensorflow.python import pywrap_tensorflow

tf.set_random_seed(228)
# import numpy as np  
# np.set_printoptions(threshold=np.inf)


def write(f, key, tensor):
	shape=tensor.shape
	f.write(struct.pack('h',len(shape)))
	f.write(struct.pack('h'+str(len(key))+'s', len(key), key))
	if len(shape) == 1:
		f.write(struct.pack('h',shape[0]))
		for x in xrange(0,shape[0]):
			byte = struct.pack('f',tensor[x])
			f.write(byte)
	elif len(shape) == 4:
		f.write(struct.pack('h',shape[2]))
		f.write(struct.pack('h',shape[3]))
		f.write(struct.pack('h',shape[0]))
		f.write(struct.pack('h',shape[1]))
		
		for i in xrange(0,shape[2]): #input count
			for j in xrange(0,shape[3]): #output count
				for k in xrange(0,shape[0]):
					for l in xrange(0,shape[1]):
						byte = struct.pack('f',tensor[k,l,i,j])
						f.write(byte)

def movefile(srcfile,dstfile):
    if not os.path.isfile(srcfile):
        print "%s not exist!"%(srcfile)
    else:
        fpath,fname=os.path.split(dstfile)    #分离文件名和路径
        if not os.path.exists(fpath):
            os.makedirs(fpath)                #创建路径
        shutil.move(srcfile,dstfile)          #移动文件
        print "move %s -> %s"%( srcfile,dstfile)
		

# pwd = os.getcwd()
# father_path=os.path.abspath(os.path.dirname(pwd)+os.path.sep+".")
# print(pwd)
# print(father_path)

checkpoint_path ="./models/model_gauguin/checkpoint_long/model11_gauguin_bks10_flw100_90000.ckpt-90000" #model11_gauguin_bks10_flw100_90000.ckpt-90000
reader = pywrap_tensorflow.NewCheckpointReader(checkpoint_path)
var_to_shape_map = reader.get_variable_to_shape_map()
print("len: ",len(var_to_shape_map))
counter = 0
f = open("args.bytes",'w')
llist = []
for key in var_to_shape_map:
    shape = reader.get_tensor(key).shape
    if not key.endswith("Adam_1") and not key.endswith("Adam") and not key.startswith("discriminator") and len(shape) > 0:
        tensor = str(reader.get_tensor(key))
        line = "public float"+str(list(shape))+" " + key.replace("/","_") + " = \n" + tensor + "\n\n"
        # print(key.replace("/","_"),shape)
        llist.append(key.replace("/","_")+"  "+str(shape))
        for x in shape:
            counter += x
        write(f, key.replace("/","_"), reader.get_tensor(key))
        tensor = reader.get_tensor(key)

llist.sort()
for x in llist:
	print x
f.write(struct.pack('h',0)) #write 0 stands for eof
print("network arg memory: %dMB"%((counter * 4)/1024))
f.close()


