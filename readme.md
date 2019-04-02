
# Shader Nearual Network

The project aimed to transfer style  realtime in the video game. 

We trained the model in the tensorflow, and forward the network in the unity environment.


## Environment

Unity2018.2 

Python2.7 or 3.5 

Tensorflow 1.7 or new


## Export Data

run the command like this:

```sh
cd python
python export.py
```

You will find *args.bytes* generated in the unity/Assets/Resources/ directory. 

The file is store arguments about neural network trained in the tensorflow.

The file format is not protobuf, defined self.


## Active Function

open this project with unity2018.2, then you can see all active function implments in the scene named ActiveFunction.

Run the unity, and click the button named *Active function*, you will see the behaviour like this:

<br><img src='image/model1.jpg'><br>

We drawed the 3 kinds of active function used R G B chanel.

R stands for relu, G stands for sigmod, while B stands for tanh.


## Contact

Email: peng_huailiang@qq.com

Blog:  https://huailiang.github.io/