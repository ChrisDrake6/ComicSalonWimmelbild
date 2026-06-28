using System.Linq;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private float requestThreshholdModifier;
    [SerializeField] private float bubbleDuration;

    private Sprite[] emojies;
    private float requestCount = 0;

    public static ConversationManager Instance { get; private set; }

    public ConversationManager()
    {
        Instance = this;
    }

    void Start()
    {
        emojies = Resources.LoadAll<Sprite>("emojis-x2-64x64");
    }    

    public bool RequestConversation()
    {
        requestCount++;
        if (requestCount >= SpawnManager.Instance.registeredSprites.Count / requestThreshholdModifier)
        {
            requestCount = 0;
            return true;
        }
        return false;
    }

    public void ShowBubble(PlayerFigureController requester)
    {
        requester.ShowBubble(emojies[Random.Range(0, emojies.Length)]);
        requester.Invoke(nameof(requester.HideBubble), bubbleDuration);
    }
}
