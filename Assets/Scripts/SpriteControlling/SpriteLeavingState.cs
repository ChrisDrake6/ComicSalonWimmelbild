using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SpriteLeavingState : SpriteBaseState
{
    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;

    public SpriteLeavingState(Animator animator, float timeOut)
    {
        this.animator = animator;
        this.timeOut = timeOut;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        if (sprite.agent.isStopped)
        {
            sprite.agent.isStopped = false;
        }
        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);

        if (sprite.isInGroup)
        {
            GroupManager.Instance.RemoveFromGroup(sprite);
        }

        int areaMask = sprite.agent.areaMask;
        areaMask |= 1 << NavMesh.GetAreaFromName("Entrance");
        sprite.agent.areaMask = areaMask;
        sprite.agent.avoidancePriority = 0;

        currentDestination = NavigationManager.Instance.GetClosestSpawnPosition(sprite.transform);
        sprite.agent.SetDestination(currentDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (sprite.agent.remainingDistance <= sprite.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            sprite.Despawn();
        }
    }

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(sprite.transform.position, currentDestination, Color.blue);
    }
}
