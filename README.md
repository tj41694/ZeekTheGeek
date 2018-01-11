# ZeekTheGeek(经典食人花) #
***

>关于：打算学习Unity3D。记得食人花是小学微机课电脑上装的一款游戏，非常适合练习，于是打算用Unity自己实现一下。

	游戏除了炸弹爆炸的prefb，蘑菇的粒子效果贴图，其他都是自己实现的，包括所有的模型、动作、scripts。

##遇到的坑
* player的移动
	* 移动前先用Physics.Linecast()函数判断能都移动，所以刚开始必须设置好layermask。
	* 为了实现平滑的单位移动，创建了一个Coroutine协程函数Smoothmove()，测试有问题，发现按下移动键**触发了多个协程函数Smoothmove()**。总结：应该设置一个移动中标记，如果是在移动中就不触发移动指令。
* player的旋转
	* 同样自己实现了平滑旋转，这里另一个坑是，创建四元数角度Quaternion.Euler(euler)时，euler值不能为负，应保证角度为[0 -360]。
* 尽量不要使用GameObject.Find(),不要问我为什么。