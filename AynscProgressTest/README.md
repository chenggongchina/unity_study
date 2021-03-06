# 使用异步或协程的游戏流程控制写法DEMO

在控制游戏流程中，我们会经常使用状态机。但状态机的一般写法会将代码封装到若干个状态中，然后进行状态迁移调用，使用起来很不直观。

作为程序员来说，写一个流程控制最好的办法还是一段代码。那么怎样比较优雅的在UI控制主逻辑（实际为Unity主线程，因为部分状态我们需要由玩家输入，比如点击一个按钮来驱动进行下一步）以外来实现它？

这里我们尝试一种基于异步编程的写法，本DEMO提供基于协程（Coroutine）和基于异步编程（Unitask）两种写法，

核心为你可以使用`WaitUntil`或者`UniTask.WaitUntil`逻辑来实现等待玩家进行输入，以驱动后续逻辑流程。

而你的主循环也可以写在一段代码里，这样整个编写非常直观，对程序员友好。

关于本方法的进一步使用，亦可参考笔者在开源游戏[金庸群侠传3D重制版战斗部分的实现](https://github.com/jynew/jynew/blob/main/jyx2/Assets/Scripts/BattleManager/BattleLoop.cs)


