using UnityEngine;
using UnityEngine.AI;

public class SpriteIdleState : SpriteBaseState
{
    float minIdleTime;
    float maxIdleTime;
    Animator animator;

    float nextRoamingTime;

    public SpriteIdleState(float minIdleTime, float maxIdleTime, Animator animator)
    {
        this.minIdleTime = minIdleTime;
        this.maxIdleTime = maxIdleTime;
        this.animator = animator;
    }

    public override void EnterState(SpriteStateManager sprite)
    {
        sprite.agent.avoidancePriority = 99;

        animator.SetBool("IsWalking", false);
        if (sprite.isInGroup)
        {
            nextRoamingTime = GroupManager.Instance.GetGroupIdleDeadLine(sprite, minIdleTime, maxIdleTime);
        }
        else
        {
            float randomIdleTime = Random.Range(minIdleTime, maxIdleTime);
            nextRoamingTime = randomIdleTime + Time.time;
        }
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (sprite.agent.remainingDistance > sprite.agent.stoppingDistance)
        {
            sprite.roamingState.emergencySwitch = true;
            sprite.SwitchState(sprite.roamingState);
        }
        if (Time.time >= nextRoamingTime)
        {
            sprite.SwitchState(sprite.roamingState);
        }
    }

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
    }
}
