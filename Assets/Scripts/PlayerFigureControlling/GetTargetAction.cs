using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Target", story: "[Agent] finds [Target] in proximity", category: "Action", id: "894157b4b74a206578c1080ff88ba2e0")]
public partial class GetTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Target;

    protected override Status OnStart()
    {
        Target.Value = NavigationManager.Instance.GetRandomShortDistanceDestination(Agent.Value);
        return Status.Success;
    }
}

