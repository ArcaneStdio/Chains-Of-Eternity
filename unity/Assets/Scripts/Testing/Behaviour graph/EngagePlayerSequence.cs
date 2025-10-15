using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Engage Player", category: "Flow", id: "699cfbc5a34fa63ed66b3a009fdba7f3")]
public partial class EngagePlayerSequence : Composite
{
    [SerializeReference] public Node Port1;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

