using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

public class SpriteStateManager : MonoBehaviour
{
    [SerializeField] float minIdleTime;
    [SerializeField] float maxIdleTime;

    [SerializeField] float shortDistanceTimeOut;
    [SerializeField] float longDistanceTimeOut;

    [SerializeField] GameObject hoverOverIndicator;

    Animator animator;

    [SerializeField] Animation talkingAnimation;
    [SerializeField] float talkingTime;
    [SerializeField] float talkingDelay;

    public SpriteRenderer EmojiContainer;
    public GameObject SpeechBubble;

    public NavMeshAgent agent;
    public bool isInGroup;

    SpriteBaseState currentState;
    public SpriteIdleState idleState;
    public SpriteRoamingState roamingState;
    public SpriteArrivingState arrivingState;
    public SpriteLeavingState leavingState;
    public SpriteTalkingState talkingState;

    public SpriteDataContainer data;

    bool stateSwitched;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();

        arrivingState = new SpriteArrivingState(animator, longDistanceTimeOut, this);
        idleState = new SpriteIdleState(minIdleTime, maxIdleTime, animator, this);
        roamingState = new SpriteRoamingState(animator, shortDistanceTimeOut, this);
        leavingState = new SpriteLeavingState(animator, longDistanceTimeOut, this);
        talkingState = new SpriteTalkingState(animator, talkingTime, talkingDelay, this);

        SwitchState(arrivingState);
    }

    void Update()
    {
        if (stateSwitched)
        {
            currentState?.UpdateState();
        }
    }

    public void SwitchState(SpriteBaseState state)
    {
        // TODO: Make this a coroutine.
        stateSwitched = false;
        currentState?.LeaveState();
        currentState = state;
        state.EnterState();
        stateSwitched = true;
    }

    public void Despawn()
    {
        SpriteRenderer bodyRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer headRenderer = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        Sprite bodySprite = bodyRenderer.sprite;
        Sprite headSprite = headRenderer.sprite;

        Texture2D bodyTex = bodySprite.texture;
        Texture2D headTex = headSprite.texture;

        Destroy(bodyTex);
        Destroy(headTex);
        Destroy(bodySprite);
        Destroy(headSprite);
        Destroy(gameObject);
    }

    private void OnMouseUp()
    {
        if (!GameManager.Instance.Paused)
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
            hoverOverIndicator.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (LinkLine.Instance.SelectedSprite == this)
        {
            LinkLine.Instance.SelectedSprite = null;
        }
        hoverOverIndicator.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState?.OnTriggerEnter(collision);
    }

    public void HideBubble()
    {
        SpeechBubble.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        currentState?.OnDrawGizmos();

        if (isInGroup)
        {
            GroupData currentGroup = GroupManager.Instance.GetCurrentGroup(this);
            if (currentGroup != null)
            {
                foreach (SpriteStateManager member in currentGroup.Members)
                {
                    Debug.DrawLine(transform.position, member.gameObject.transform.position, Color.green);
                }
            }
        }

        //Gizmos.DrawWireSphere(transform.position, NavigationManager.Instance.RoamingRadius);
    }
}
