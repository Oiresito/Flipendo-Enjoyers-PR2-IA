using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : BehaviorNode
{
    private List<BehaviorNode> nodes = new List<BehaviorNode>();

    public void AddNode(BehaviorNode node)
    {
        nodes.Add(node);
    }

    public override NodeStatus Execute()
    {
        foreach (var node in nodes)
        {
            NodeStatus result = node.Execute();

            if (result != NodeStatus.Success)
            {
                return result;
            }
        }

        return NodeStatus.Success;
    }
}
