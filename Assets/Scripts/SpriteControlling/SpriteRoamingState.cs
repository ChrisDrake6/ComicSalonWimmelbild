using UnityEngine;

public class SpriteRoamingState : SpriteBaseState
{
    public bool emergencySwitch = false;

    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;


    public SpriteRoamingState(Animator animator, float timeOut)
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
        sprite.agent.avoidancePriority = Random.Range(0, 99);

        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);

        if (!emergencySwitch)
        {
            if (sprite.isInGroup)
            {
                currentDestination = GroupManager.Instance.GetCurrentGroupDestination(sprite);
            }
            else
            {
                currentDestination = NavigationManager.Instance.GetRandomShortDistanceDestination(sprite.transform);
            }
        }
        else
        {
            emergencySwitch = false;
        }
        sprite.agent.SetDestination(currentDestination);
    }

    public override void UpdateState(SpriteStateManager sprite)
    {        
        if (sprite.agent.remainingDistance <= sprite.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            sprite.SwitchState(sprite.idleState);
            if (Time.time >= currentTimeOut)
            {
                sprite.agent.isStopped = true;
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
