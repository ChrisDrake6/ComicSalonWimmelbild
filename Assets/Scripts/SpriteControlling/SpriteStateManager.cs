using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

public class SpriteStateManager : MonoBehaviour
{
    [SerializeField] float minIdleTime;
    [SerializeField] float maxIdleTime;

    [SerializeField] float roamingRadius;
    [SerializeField] float arrivalLeeway;

    [SerializeField] float shortDistanceTimeOut;
    [SerializeField] float longDistanceTimeOut;

    [SerializeField] GameObject hoverOverIndicator;

    NavMeshAgent agent;
    Animator animator;

    public SpriteRenderer EmojiContainer;
    public GameObject SpeechBubble;

    public bool isInGroup;

    SpriteBaseState currentState;
    public SpriteIdleState idleState;
    public SpriteRoamingState roamingState;
    public SpriteArrivingState arrivingState;
    public SpriteLeavingState leavingState;
    public SpriteDataContainer data;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 99);

        animator = GetComponent<Animator>();

        arrivingState = new SpriteArrivingState(agent, animator, longDistanceTimeOut, arrivalLeeway);
        idleState = new SpriteIdleState(minIdleTime, maxIdleTime, animator);
        roamingState = new SpriteRoamingState(roamingRadius, agent, animator, shortDistanceTimeOut, arrivalLeeway);
        leavingState = new SpriteLeavingState(agent, animator, longDistanceTimeOut, arrivalLeeway);

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

    public void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (Time.timeScale != 0)
        {
            RadialMenuManager.Instance.OnSpriteClick(this);
            hoverOverIndicator.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if (Time.timeScale != 0)
        {
            hoverOverIndicator.SetActive(true);
        }
        if (LinkLine.Instance.IsActive())
        {
            LinkLine.Instance.SelectedSprite = this;
        }
    }

    private void OnMouseExit()
    {
        hoverOverIndicator.SetActive(false);
    }

    public void HideBubble()
    {
        SpeechBubble.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        currentState?.OnDrawGizmos(this);
        if (isInGroup)
        {
            GroupData currentGroup = GroupManager.Instance.GetCurrentGroup(this);
            foreach (SpriteStateManager member in currentGroup.Members)
            {
                Debug.DrawLine(transform.position, member.gameObject.transform.position, Color.green);
            }
        }

        //Gizmos.DrawWireSphere(transform.position, roamingRadius);
    }
}
