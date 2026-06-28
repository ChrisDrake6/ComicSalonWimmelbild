using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PutOnHat", story: "[Figure] puts on hat", category: "Action", id: "16a6a9aad368ec3a2cf35fdfd84eba31")]
public partial class PutOnHatAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerFigureController> Figure;

    protected override Status OnStart()
    {
        Figure.Value.PutOnHat();
        return Status.Success;
    }
}

