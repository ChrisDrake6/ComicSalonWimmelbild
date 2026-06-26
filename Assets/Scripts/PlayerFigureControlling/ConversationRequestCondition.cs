using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ConversationRequest", story: "[Agent] can begin a conversation with [CollidedObject]", category: "Conditions", id: "a95d2c402e4ab63a38e4cc583d8a240d")]
public partial class ConversationRequestCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> CollidedObject;

    public override bool IsTrue()
    {
        if (CollidedObject.Value == null)
        {
            return false;
        }
        if(!CollidedObject.Value.CompareTag("Player"))
        {
            return false;
        }
        if (ConversationManager.Instance.RequestConversation())
        {
            return true;
        }
        return false;
    }
}
