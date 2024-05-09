using UnityEngine;
using UnityEngine.AI;

public class SpriteStateManager : MonoBehaviour
{
    [SerializeField] float minIdleTime;
    [SerializeField] float maxIdleTime;

    [SerializeField] float roamingRadius;

    [SerializeField] float walkingTimeOut;

    NavMeshAgent agent;
    Animator animator;

    SpriteBaseState currentState;
    public SpriteIdleState idleState;
    public SpriteRoamingState roamingState;
    public SpriteArrivingState arrivingState;
    public SpriteLeavingState leavingState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 99);

        animator = GetComponent<Animator>();

        arrivingState = new SpriteArrivingState(agent, animator, walkingTimeOut);
        idleState = new SpriteIdleState(minIdleTime, maxIdleTime, animator);
        roamingState = new SpriteRoamingState(roamingRadius, agent, animator, walkingTimeOut);
        leavingState = new SpriteLeavingState();

        SwitchState(arrivingState);
    }

    void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(SpriteBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    private void OnDrawGizmos()
    {
        currentState?.OnDrawGizmos(this);
        //Gizmos.DrawWireSphere(transform.position, roamingRadius);
    }
}
