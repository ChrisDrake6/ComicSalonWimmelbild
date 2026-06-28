using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFigureController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer emojiContainer;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Transform hatContainer;

    private Hat _currentHat;
    private NavMeshAgent _navAgent;
    private BehaviorGraphAgent _graphAgent;

    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false;
        _navAgent.updateUpAxis = false;
        _navAgent.avoidancePriority = Random.Range(0, 99);

        _graphAgent = GetComponent<BehaviorGraphAgent>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsInteractable() && collision.gameObject.GetComponent<PlayerFigureController>().IsInteractable())
            {
                // This needs to be in its own if!
                if (ConversationManager.Instance.RequestConversation())
                {
                    _graphAgent.SetVariableValue("FigureState", FigureState.Talking);
                    collision.gameObject.GetComponent<BehaviorGraphAgent>().SetVariableValue("FigureState", FigureState.Talking);
                }
            }
        }
    }

    public void ShowBubble(Sprite emoji)
    {
        speechBubble.gameObject.SetActive(true);
        emojiContainer.sprite = emoji;
    }

    public void HideBubble()
    {
        speechBubble.gameObject.SetActive(false);
    }

    public bool IsInteractable()
    {
        BlackboardVariable<FigureState> figureState;
        if (!_graphAgent.GetVariable("FigureState", out figureState))
        {
            return false;
        }
        switch (figureState.Value)
        {
            case FigureState.Arriving:
            case FigureState.Leaving:
            case FigureState.Talking:
            case FigureState.GettingHat:
                return false;
            default:
                return true;
        }
    }

    public void AssignHat(Hat hat)
    {
        _currentHat = hat;
        _graphAgent.SetVariableValue("FigureState", FigureState.GettingHat);
    }

    public Hat GetCurrentHat()
    {
        return _currentHat;
    }

    public void PutOnHat()
    {
        if (_currentHat != null)
        {
            _currentHat.transform.SetParent(hatContainer);
            _currentHat.OnPickUp();
        }
    }

    public void Despawn()
    {
        SpriteRenderer bodyRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer headRenderer = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        Sprite bodySprite = bodyRenderer.sprite;
        Sprite headSprite = headRenderer.sprite;

        Texture2D bodyTex = bodySprite.texture;
        Texture2D headTex = headSprite.texture;

        if (_currentHat != null)
        {
            _currentHat?.GetComponent<Hat>().SelfDestruct();
        }

        Destroy(bodyTex);
        Destroy(headTex);
        Destroy(bodySprite);
        Destroy(headSprite);
        Destroy(gameObject);
    }
}
