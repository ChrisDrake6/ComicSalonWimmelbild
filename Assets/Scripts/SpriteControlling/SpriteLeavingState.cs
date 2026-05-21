using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SpriteLeavingState : SpriteBaseState
{
    Vector3 currentDestination;
    Animator animator;
    float timeOut;
    float currentTimeOut;
    private SpriteStateManager _stateManager;

    public SpriteLeavingState(Animator animator, float timeOut, SpriteStateManager stateManager)
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
        currentTimeOut = Time.time + timeOut;
        animator.SetBool("IsWalking", true);

        if (_stateManager.IsInGroup)
        {
            GroupManager.Instance.RemoveFromGroup(_stateManager);
        }
        SpawnManager.Instance.registeredSprites.Remove(SpawnManager.Instance.registeredSprites.Single(a => a.AssignedPrefab == _stateManager.gameObject));
        int areaMask = _stateManager.agent.areaMask;
        areaMask |= 1 << NavMesh.GetAreaFromName("Entrance");
        _stateManager.agent.areaMask = areaMask;
        _stateManager.agent.avoidancePriority = 0;

        currentDestination = NavigationManager.Instance.GetClosestSpawnPosition(_stateManager.transform);
        _stateManager.agent.SetDestination(currentDestination);

        _stateManager.data.PresentOnScene = false;
    }

    public override void UpdateState()
    {
        if (Vector3.Distance(_stateManager.transform.position, currentDestination) <= _stateManager.agent.stoppingDistance || Time.time >= currentTimeOut)
        {
            _stateManager.Despawn();
        }
    }

    public override void LeaveState() { }

    public override void OnTriggerEnter(Collider2D collision) { }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
        Debug.DrawLine(_stateManager.transform.position, currentDestination, Color.blue);
    }
}
