using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetLongDistanceTarget", story: "[Agent] finds [Target] on Map", category: "Action", id: "682b879a1021c71932e60f2c17c3f6ab")]
public partial class GetLongDistanceTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Target;

    protected override Status OnStart()
    {
        Target.Value = NavigationManager.Instance.GetRandomLongDistanceDestination(Agent.Value);
        return Status.Success;
    }
}

