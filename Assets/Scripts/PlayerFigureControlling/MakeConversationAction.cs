using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MakeConversation", story: "[Agent] makes conversation", category: "Action", id: "258268c45204cd734e836aa3addbbcba")]
public partial class MakeConversationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private PlayerFigureController _controller;
    private Animator _animator;

    protected override Status OnStart()
    {
        _controller = Agent.Value.GetComponent<PlayerFigureController>();
        ConversationManager.Instance.ShowBubble(_controller);
        _animator = Agent.Value.GetComponent<Animator>();
        _animator.Play("DoubleJump_Quick");
        return Status.Success;
    }
}

