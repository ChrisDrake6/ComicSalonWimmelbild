using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTarget", story: "[Agent] moves to [Target]", category: "Action", id: "78996b334b4473b29facbad2f080cdb0")]
public partial class MoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Target;

    protected override Status OnStart()
    {
        Agent.Value.avoidancePriority = UnityEngine.Random.Range(0, 99);
        Agent.Value.SetDestination(Target.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Agent.Value.remainingDistance <= Agent.Value.stoppingDistance)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Agent.Value.isStopped = true;
    }
}

