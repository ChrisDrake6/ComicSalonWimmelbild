using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] float requestThreshholdModifier;
    [SerializeField] float bubbleDuration;

    Sprite[] emojies;
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

    public void RequestConversation(SpriteStateManager requester, SpriteStateManager partner)
    {
        requestCount++;
        if (requestCount >= SpawnManager.Instance.registeredSprites.Count / requestThreshholdModifier)
        {
            requester.SwitchState(requester.talkingState);
            partner.SwitchState(partner.talkingState);
            requestCount = 0;
        }
    }

    public void ShowBubble(SpriteStateManager requester)
    {
        requester.SpeechBubble.gameObject.SetActive(true);
        requester.EmojiContainer.sprite = emojies[Random.Range(0, emojies.Length)];
        requester.Invoke(nameof(requester.HideBubble), bubbleDuration);
    }
}
