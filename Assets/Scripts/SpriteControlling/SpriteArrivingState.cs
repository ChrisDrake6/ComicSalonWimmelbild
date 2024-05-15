using UnityEngine;
using UnityEngine.AI;

public class SpriteArrivingState : SpriteBaseState
{
    Vector3 initialDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;

    public SpriteArrivingState(Animator animator, float timeOut)
    {
        this.animator = animator;
        this.timeOut = timeOut;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        sprite.agent.avoidancePriority = Random.Range(0, 99);
        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);
        initialDestination = NavigationManager.Instance.GetRandomLongDistanceDestination(sprite.transform);
        sprite.agent.SetDestination(initialDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (sprite.agent.remainingDistance <= sprite.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            sprite.SwitchState(sprite.idleState);
            int areaMask = sprite.agent.areaMask;
            areaMask -= 1 << NavMesh.GetAreaFromName("Entrance");
            sprite.agent.areaMask = areaMask;

            if (Time.time >= currentTimeOut)
            {
                sprite.agent.isStopped = true;
            }
        }
    }

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(sprite.transform.position, initialDestination, Color.blue);
    }
}
