using UnityEngine;
using UnityEngine.AI;

public class SpriteArrivingState : SpriteBaseState
{
    Vector3 initialDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    private SpriteStateManager _stateManager;

    public SpriteArrivingState(Animator animator, float timeOut, SpriteStateManager stateManager)
    {
        this.animator = animator;
        this.timeOut = timeOut;
        _stateManager = stateManager;
    }

    public override void EnterState()
    {
        _stateManager.agent.avoidancePriority = Random.Range(0, 99);
        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);
        initialDestination = NavigationManager.Instance.GetRandomLongDistanceDestination(_stateManager.transform);
        _stateManager.agent.SetDestination(initialDestination);
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
        int areaMask = _stateManager.agent.areaMask;
        areaMask -= 1 << NavMesh.GetAreaFromName("Entrance");
        _stateManager.agent.areaMask = areaMask;
        _stateManager.agent.isStopped = true;
    }

    public override void OnTriggerEnter(Collider2D collision) { }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(_stateManager.transform.position, initialDestination, Color.blue);
    }
}
