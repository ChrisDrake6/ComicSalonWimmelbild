using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetDirectionToExit", story: "[Agent] gets [direction] to exit", category: "Action", id: "4df35a1fb67501faa76377f1d6477247")]
public partial class GetDirectionToExitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Direction;

    protected override Status OnStart()
    {
        Direction.Value = NavigationManager.Instance.GetClosestSpawnPosition(Agent.Value.transform);
        Agent.Value.GetComponent<PlayerFigureController>().FigureData.PresentOnScene = false;
        return Status.Success;
    }
}

