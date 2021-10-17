# Behavior Designer学习笔记

## 相关资源

### 官方文档
https://opsive.com/support/documentation/behavior-designer/overview/

## 行为树 VS 有限状态机
https://opsive.com/support/documentation/behavior-designer/behavior-trees-or-finite-state-machines/

## 实体概念

* `Behavior Tree` 行为树，可跟Component一样挂载到GameObject上。（其中可以引用场景物体`Scene Objects`）
* `Extenal Behavior` 外部行为树（可以理解为行为树模板）， 实际是树存储为asset，无法引用场景物体。
* `Task` 树中的一个节点叫一个Task
    * `Conditional Task` 条件任务
    * `Action Task` 行为任务
    * `Composite Task` 组合任务，有多个子节点，用来做调度
    * `Decorator Task` 装饰器任务，只有一个子节点

## 关于生命周期控制

行为树插件不会自动重新执行，每次结束后需要重新的执行的话可以有两种方法：

* 增加Repeat节点（不断持续执行的话，可勾选`Repeat Forever`选项）
* 在行为树组件上勾选`Restart when Complete`

## 配置选项

### 节点上Instant参数的作用

勾选了Instant节点在同一帧中执行，否则在下一帧执行。文档原文如下

> Tasks have three exposed properties: name, comment, and instant. Instant is the only property that isn’t obvious in what it does. When a task returns success or fail it immediately moves onto the next task within the same update tick. If you uncheck the instant task it will now wait a update tick before the next task gets executed. This is an easy way to throttle the behavior tree.

## 模板实例化后的参数传递

由于Extenal Behavior保存为asset的行为树“模板”，实例化以后通常需要传递场景物体作为参数。这里有调用顺序的要求，样例如下：
（先在行为树中添加名为target的GameObject类型变量）


```c#
  ExternalBehaviorTree behaviorSource;
  GameObject player;

  ...

  //behaviorSource.SetVariableValue("target",player); //无用1，不会赋值
  var instance = Instantiate(behaviorSource);
  //instance.SetVariableValue("target", player);  //无用2，不会赋值
  
  var tree = npc.AddComponent<BehaviorTree>();
  tree.ExternalBehavior = instance; //step1: 必须先设置ExternalBehavior
  tree.SetVariableValue("target", player); //step2: 正确方法：然后给参数赋值
  tree.StartWhenEnabled = true;
```

## 一些高级应用

打断Running的方法为使用[Conditional Abort](https://opsive.com/support/documentation/behavior-designer/conditional-aborts/)
