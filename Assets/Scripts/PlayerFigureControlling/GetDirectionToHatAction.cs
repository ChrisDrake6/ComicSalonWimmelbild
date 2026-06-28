using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetDirectionToHat", story: "[Figure] gets [direction] to hat", category: "Action", id: "98c15628920b1ed9ac01ea9f78b3bb04")]
public partial class GetDirectionToHatAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerFigureController> Figure;
    [SerializeReference] public BlackboardVariable<Vector2> Direction;
    protected override Status OnStart()
    {
        Direction.Value = Figure.Value.GetCurrentHat().transform.position;
        return Status.Success;
    }
}

