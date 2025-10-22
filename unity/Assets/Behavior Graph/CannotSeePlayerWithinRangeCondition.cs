using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "cannot see player within range", story: "[Agent] is not in proximity to enemy", category: "Conditions/Enemy", id: "cf022587fce9622d3d06a2c2dae80903")]
public partial class CannotSeePlayerWithinRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
