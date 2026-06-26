using System.Threading;
using UnityEngine;

public class SpriteGetHatState : SpriteBaseState
{
    Animator animator;
    float timeOut;
    float currentTimeOut;
    private SpriteStateManager _stateManager;
    private Transform _head;

    public SpriteGetHatState(Animator animator, float timeOut, SpriteStateManager stateManager, Transform head)
    {
        this.animator = animator;
        this.timeOut = timeOut;
        _stateManager = stateManager;
        _head = head;
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
        _stateManager.agent.SetDestination(_stateManager.CurrentHat.transform.position);
    }

    public override void UpdateState()
    {
        if (_stateManager.agent.remainingDistance <= _stateManager.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            _stateManager.SwitchState(_stateManager.roamingState);
        }
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
    }

    public override void LeaveState()
    {
        _stateManager.CurrentHat.transform.SetParent(_head);
        _stateManager.CurrentHat.GetComponent<Hat>().OnPickUp();
        animator.SetBool("IsWalking", false);
        _stateManager.agent.isStopped = true;
    }
    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(_stateManager.transform.position, _stateManager.CurrentHat.transform.position, Color.blue);
    }
}
