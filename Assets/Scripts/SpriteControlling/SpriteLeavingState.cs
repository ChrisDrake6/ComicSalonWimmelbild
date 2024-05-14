using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SpriteLeavingState : SpriteBaseState
{
    NavMeshAgent agent;
    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    float arrivalLeeway;

    public SpriteLeavingState(NavMeshAgent agent, Animator animator, float timeOut, float arrivalLeeway)
    {
        this.agent = agent;
        this.animator = animator;
        this.timeOut = timeOut;
        this.arrivalLeeway = arrivalLeeway;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);

        if (sprite.isInGroup)
        {
            GroupManager.Instance.RemoveFromGroup(sprite);
        }

        int areaMask = agent.areaMask;
        areaMask |= 1 << NavMesh.GetAreaFromName("Entrance");
        agent.areaMask = areaMask;
        agent.avoidancePriority = 0;

        GameObject closestSpawnPoint = SpawnManager.Instance.spawnPoints.OrderBy(a => Vector3.Distance(sprite.transform.position, a.transform.position)).First();
        currentDestination = closestSpawnPoint.transform.position;
        agent.SetDestination(currentDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (Vector3.Distance(sprite.transform.position, currentDestination) <= arrivalLeeway || Time.time >= currentTimeOut)
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
