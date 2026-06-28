using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Despawn", story: "[Figure] leaves scene", category: "Action", id: "f7361f76321e639230359b6fc9a91b37")]
public partial class DespawnAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerFigureController> Figure;
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

