
# 基于神经网络的风格转换

[English Version](./readme.md)

[paper][i1]

[website][i2]

此项目旨在视频游戏中转换风格。

我们使用预训练好的TensorFlow模型， 并在unity使用compute shader实现了一套前向传播的网络。

<br><img src='image/tf1.jpg'> <br>


<br><img src='image/tf2.jpg'> <br>

如上图所示， 左边是原图，中间是unity转换的图片，右边是TensorFlow inrefrence的图片。

## 环境

Unity2018.2 	<br>
Python2.7 or 3.5 <br>
Tensorflow 1.7 or new <br>
PIL, numpy, scipy, cv2 <br>
tqdm

## 数据处理

### 导出网络参数命令:

```sh
python main.py \
         --model_name=model_van-gogh \
         --phase=export_arg \
         --image_size=256
```

运行成功之后，在unity/Assets/Resources/目录中可以发现 *args.bytes* .

此配置文件包含了TensorFlow中训练的模型参数。注意这个不是protobuf格式的，而是自定义的数据格式。

生成bytes之后， 在unity中菜单栏点击Tools->GenerateMap， 生成map.asset, 然后选中map, 在Inspector中点击Analysis,可以生成参数和compute shader kernel的映射表， 如下图所示。


<br><img src='image/model3.jpg'><br>

点击save按钮，map会序列化到磁盘中。


###  导出网络layer，运行命令:

```sh
python main.py \
         --model_name=model_van-gogh \
         --phase=export_layers \
         --image_size=256
```


你也可以在[百度云盘](https://pan.baidu.com/s/13_kSWE-OiqHFDXix9NwL_g)上下载到这些导出好的数据, 然后导入到unity.


导出之后，在unity中运行Tools->LayerVisual, 可以看到每一层的数据可视化图像，如下图所示：

<br><img src='image/model2.jpg'><br>

### python环境中预览模型效果:


```sh
python main.py \
         --model_name=model_van-gogh \
         --phase=inference \
         --image_size=256
```

运行命令后，会在model目录生成对应分辨率的图片。

## 训练

内容训练集图片 [microsoft coco dataset train mages (13GB)](http://mscoco.org).  

风格化图片 [download link](https://hcicloud.iwr.uni-heidelberg.de/index.php/s/NcJj2oLBTYuT1tf).   

1. 风格化图像下载到 `./data`.   
2. 下载内容图片
3. 训练使用命令

```
CUDA_VISIBLE_DEVICES=1 python main.py \
                 --model_name=model_van-gogh-new \
                 --batch_size=1 \
                 --phase=train \
                 --image_size=256 \
                 --lr=0.0002 \
                 --dsr=0.8 \
                 --ptcd=/data/micro_coco _dataset \
                 --ptad=./data/vincent-van-gogh_road-with-cypresses-1890
```

## 说明

在compute shader中，threadgroup 优化分配满足z方向的线程。

但是z方向的受限于最大个数64个线程， 在batch-normal阶段，我们交换了thread-x和thread-z， 因为thread-x最大可以达到1024个。

在训练期间， 参数model-name应该不等同于inference阶段， 否则的话， 网络会从老的与训练好的模型载入。


## 激励函数

在unity中，有一个单独的ActiveFunc scene, 可以预览激励函数的变化。点击按钮 *Active function*， 你就会看到如下图的效果：

<br><img src='image/model1.jpg'><br>

我们使用不同的三种颜色，代表了不同的三种不同的激励函数，R是relu， G是sigmod, B是tanh。


## 联系方式

Email: peng_huailiang@qq.com

Blog:  https://huailiang.github.io/



[i1]: https://github.com/huailiang/nnStyle/blob/master/image/paper.pdf
[i2]: https://huailiang.github.io/blog/2019/nnstyle/