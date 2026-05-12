using UnityEngine;

public class SpriteRoamingState : SpriteBaseState
{
    public bool emergencySwitch = false;

    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    private SpriteStateManager _stateManager;

    public SpriteRoamingState(Animator animator, float timeOut, SpriteStateManager stateManager)
    {
        this.animator = animator;
        this.timeOut = timeOut;
        _stateManager = stateManager;
    }

    public override void EnterState()
    {
        if (_stateManager.agent.isStopped)
        {
            _stateManager.agent.isStopped = false;
        }
        _stateManager.agent.avoidancePriority = Random.Range(0, 99);

        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);

        if (!emergencySwitch)
        {
            if (_stateManager.isInGroup)
            {
                currentDestination = GroupManager.Instance.GetCurrentGroupDestination(_stateManager);
            }
            else
            {
                currentDestination = NavigationManager.Instance.GetRandomShortDistanceDestination(_stateManager.transform);
            }
        }
        else
        {
            emergencySwitch = false;
        }
        _stateManager.agent.SetDestination(currentDestination);
    }

    public override void UpdateState()
    {
        if (_stateManager.agent.remainingDistance <= _stateManager.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            _stateManager.SwitchState(_stateManager.idleState);
            
        }
    }

    public override void LeaveState()
    {
        animator.SetBool("IsWalking", false);
        //if (Time.time >= currentTimeOut)
        //{
            _stateManager.agent.isStopped = true;
        //}
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        if (collision != null && !_stateManager.isInGroup)
        {
            SpriteStateManager partner = collision.gameObject.GetComponent<SpriteStateManager>();
            if (partner != null && !partner.isInGroup)
            {
                ConversationManager.Instance.RequestConversation(_stateManager, partner);
            }
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(_stateManager.transform.position, currentDestination, Color.blue);
    }
}
