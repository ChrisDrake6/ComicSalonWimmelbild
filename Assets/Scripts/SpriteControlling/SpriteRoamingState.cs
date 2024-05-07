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

    public SpriteRoamingState(float roamingRadius, NavMeshAgent agent, Animator animator, float timeOut)
    {
        this.roamingRadius = roamingRadius;
        this.agent = agent;
        this.animator = animator;
        this.timeOut = timeOut;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        if (sprite.debugStatusDisplay.enabled)
        {
            sprite.debugStatusDisplay.color = Color.yellow;
        }

        if (agent.isStopped)
        {
            agent.isStopped = false;
        }

        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);
        Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
        randomDirection += sprite.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamingRadius, 1);
        currentDestination = hit.position;
        agent.SetDestination(currentDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (sprite.transform.position == currentDestination || Time.time >= currentTimeOut)
        {
            sprite.SwitchState(sprite.idleState);
            if (Time.time >= currentTimeOut)
            {
                agent.isStopped = true;
            }
        }
    }
}
