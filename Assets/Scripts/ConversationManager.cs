using System.Linq;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private float requestThreshholdModifier;
    [SerializeField] private float bubbleDuration;

    private Sprite[] _emojies;
    private float _requestCount = 0;

    public static ConversationManager Instance { get; private set; }

    public ConversationManager()
    {
        Instance = this;
    }

    void Start()
    {
        _emojies = Resources.LoadAll<Sprite>("emojis-x2-64x64");
    }    

    public bool RequestConversation()
    {
        _requestCount++;
        if (_requestCount >= SpawnManager.Instance.RegisteredFigures.Count / requestThreshholdModifier)
        {
            _requestCount = 0;
            return true;
        }
        return false;
    }

    public void ShowBubble(PlayerFigureController requester)
    {
        requester.ShowBubble(_emojies[Random.Range(0, _emojies.Length)]);
        requester.Invoke(nameof(requester.HideBubble), bubbleDuration);
    }
}
