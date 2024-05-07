using UnityEngine;

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
        if(sprite.debugStatusDisplay.enabled)
        {
            sprite.debugStatusDisplay.color = Color.green;
        }

        animator.SetBool("IsWalking", false);
        float randomIdleTime = Random.Range(minIdleTime, maxIdleTime);
        nextRoamingTime = randomIdleTime + Time.time;
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
        if (Time.time >= nextRoamingTime)
        {
            sprite.SwitchState(sprite.roamingState);
        }
    }
}
