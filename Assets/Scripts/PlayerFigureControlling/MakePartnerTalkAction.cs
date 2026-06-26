using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MakePartnerTalk", story: "Make [CollidedObject] talk", category: "Action", id: "39139b49d77e42e70d582f633b9aebb3")]
public partial class MakePartnerTalkAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> CollidedObject;

    protected override Status OnStart()
    {
        CollidedObject.Value.GetComponent<BehaviorGraphAgent>().SetVariableValue("IsTalking", true);
        return Status.Success;
    }   
}

