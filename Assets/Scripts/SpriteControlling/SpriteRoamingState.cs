using UnityEngine;
using UnityEngine.AI;

public class SpriteRoamingState : SpriteBaseState
{
    float roamingRadius;
    NavMeshAgent agent;
    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    float arrivalLeeway;

    public SpriteRoamingState(float roamingRadius, NavMeshAgent agent, Animator animator, float timeOut, float arrivalLeeway)
    {
        this.roamingRadius = roamingRadius;
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

        Vector3 randomDirection = Vector3.zero;

        if (sprite.isInGroup)
        {
            currentDestination = GroupManager.Instance.GetCurrentGroupDestination(sprite, roamingRadius);
        }
        else
        {
            randomDirection = Random.insideUnitSphere * roamingRadius;
            randomDirection += sprite.transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamingRadius, 1);
            currentDestination = hit.position;
        }
        agent.SetDestination(currentDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (Vector3.Distance(sprite.transform.position, currentDestination) <= arrivalLeeway || Time.time >= currentTimeOut)
        {
            sprite.SwitchState(sprite.idleState);
            if (Time.time >= currentTimeOut)
            {
                agent.isStopped = true;
            }
        }
    }

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(sprite.transform.position, currentDestination, Color.blue);
    }
}
