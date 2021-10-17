using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Test")]
public class BehaviorTestCond : Conditional
{
    public int Type;
    
    private BehaviorTestRoot root;
    public override void OnAwake()
    {
        root = GameObject.FindObjectOfType<BehaviorTestRoot>();
    }

    public override TaskStatus OnUpdate()
    {
        if (root.behaviorType == Type)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }

}
