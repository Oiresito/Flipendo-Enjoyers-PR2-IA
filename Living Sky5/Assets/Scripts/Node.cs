using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode
{
    public abstract NodeStatus Execute();
}

public enum NodeStatus
{
    Success,
    Failure,
    Running
}

