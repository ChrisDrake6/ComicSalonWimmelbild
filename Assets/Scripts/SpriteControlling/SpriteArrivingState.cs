using UnityEngine;
using UnityEngine.AI;

public class SpriteArrivingState : SpriteBaseState
{
    Vector3 initialDestination;
    NavMeshAgent agent;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    float arrivalLeeway;

    public SpriteArrivingState(NavMeshAgent agent, Animator animator, float timeOut, float arrivalLeeway)
    {
        this.agent = agent;
        this.animator = animator;
        this.timeOut = timeOut;
        this.arrivalLeeway = arrivalLeeway;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        currentTimeOut = Time.time + timeOut + 10;
        animator.SetBool("IsWalking", true);
        Vector3 randomPoint = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f ), 10);
        Vector3 randomDirection = Camera.main.ViewportToWorldPoint(randomPoint);
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, Camera.main.pixelWidth, 1);
        initialDestination = hit.position;
        agent.SetDestination(initialDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (Vector3.Distance(sprite.transform.position, initialDestination) <= arrivalLeeway || Time.time >= currentTimeOut)
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

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(sprite.transform.position, initialDestination, Color.blue);
    }
}
