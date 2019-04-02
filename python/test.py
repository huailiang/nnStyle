# encoding=utf8

import os
import tensorflow as tf
import struct

tf.set_random_seed(228)

# 输出checkpoint里的参数

# from tensorflow.python import pywrap_tensorflow
# checkpoint_path ="./models/model_gauguin/checkpoint_long/model11_gauguin_bks10_flw100_90000.ckpt-90000" #model11_gauguin_bks10_flw100_90000.ckpt-90000
# reader = pywrap_tensorflow.NewCheckpointReader(checkpoint_path)
# var_to_shape_map = reader.get_variable_to_shape_map()
# print("len: ",len(var_to_shape_map))
# counter = 0
# llist = []
# for key in var_to_shape_map:
#     shape = reader.get_tensor(key).shape
#     if not key.endswith("Adam_1") and not key.endswith("Adam") and not key.startswith("discriminator") and len(shape) > 0:
#         tensor = str(reader.get_tensor(key))
#         print(key.replace("/","_"),shape)
#         llist.append(key.replace("/","_")+"  "+str(shape))
#         for x in shape:
#             counter += x

# llist.sort()
# for x in llist:
# 	print x
# print("network arg memory: %dMB"%((counter * 4)/1024))


img = tf.Variable(tf.ones([1,80,80,256]))
print img.get_shape()
print(img.get_shape().as_list()[-1])


# 用来测试tf.nn.moments & keep_dims

# img = tf.Variable(tf.ones([32,32,4]))
# axis=[0,1]
# mean,variance = tf.nn.moments(img,axis,keep_dims=False)
# print(img)
# print(mean)
# print(variance)

# with tf.Session() as sess:
#     sess.run(tf.global_variables_initializer())
#     resultMean = sess.run(mean)
#     print(resultMean)
#     resultMean = sess.run(variance)
#     print(resultMean)




'''
# 用来测试不同维度的tensor相加的结果
a1 = tf.constant([1,2,3])
a2 = tf.constant([1])
with tf.Session() as sess:
    sess.run(tf.global_variables_initializer())
    v = sess.run(a1+a2)
    print(v)
'''


'''

# 用来测试tf.pad
a1 = tf.constant([[2,3,4],[5,6,7]])
print(a1.get_shape())
pad = tf.pad(a1,paddings=[[1,1],[2,2]],mode="CONSTANT");
with tf.Session() as sess:
    sess.run(tf.global_variables_initializer())
    v = sess.run(pad)
    print(pad.get_shape())

'''

'''


# struct pack 打包数据到二进制


with open("hh.bytes",'wb') as f:
	a= 12
	byte = struct.pack('i',a)
	f.write(byte)
	b = 2.54
	byte = struct.pack('f',b)
	f.write(byte)
	a= 124
	byte = struct.pack('i',a)
	f.write(byte)
'''






