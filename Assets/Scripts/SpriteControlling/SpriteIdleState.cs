using UnityEngine;
using UnityEngine.AI;

public class SpriteIdleState : SpriteBaseState
{
    float minIdleTime;
    float maxIdleTime;
    Animator animator;
    private SpriteStateManager _stateManager;

    float nextRoamingTime;

    public SpriteIdleState(float minIdleTime, float maxIdleTime, Animator animator, SpriteStateManager stateManager)
    {
        this.minIdleTime = minIdleTime;
        this.maxIdleTime = maxIdleTime;
        this.animator = animator;
        _stateManager = stateManager;
    }

    public override void EnterState()
    {
        _stateManager.agent.avoidancePriority = 99;

        if (_stateManager.IsInGroup)
        {
            nextRoamingTime = GroupManager.Instance.GetGroupIdleDeadLine(_stateManager, minIdleTime, maxIdleTime);
        }
        else
        {
            float randomIdleTime = Random.Range(minIdleTime, maxIdleTime);
            nextRoamingTime = randomIdleTime + Time.time;
        }
    }

    public override void UpdateState()
    {
        if (_stateManager.agent.remainingDistance > _stateManager.agent.stoppingDistance)
        {
            _stateManager.roamingState.emergencySwitch = true;
            _stateManager.SwitchState(_stateManager.roamingState);
        }
        if (Time.time >= nextRoamingTime)
        {
            _stateManager.SwitchState(_stateManager.roamingState);
        }
    }

    public override void LeaveState() { }

    public override void OnTriggerEnter(Collider2D collision) { }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
    }
}
