using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class SpriteArrivingState : SpriteBaseState
{
    Vector3 initialDestination;
    NavMeshAgent agent;
    Animator animator;
    float timeOut;
    float currentTimeOut;

    public SpriteArrivingState(NavMeshAgent agent, Animator animator, float timeOut)
    {
        this.agent = agent;
        this.animator = animator;
        this.timeOut = timeOut;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        currentTimeOut = Time.time +timeOut;
        animator.SetBool("IsWalking", true);
        Vector3 randomPoint = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f ), 10);
        Vector3 randomDirection = Camera.main.ViewportToWorldPoint(randomPoint);
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, Camera.main.pixelHeight, 1);
        initialDestination = hit.position;
        agent.SetDestination(initialDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (sprite.transform.position == initialDestination || Time.time >= currentTimeOut)
        {
            sprite.SwitchState(sprite.idleState);
            int areaMask = agent.areaMask;
            areaMask -= 1 << NavMesh.GetAreaFromName("Entrance");
            agent.areaMask = areaMask;

            if (Time.time >= currentTimeOut)
            {
                agent.isStopped = true;
            }
        }
    }
}
