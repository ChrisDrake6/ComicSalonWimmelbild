using System.IO;
using System.Linq;
using UnityEngine;

public class EmojiManager : MonoBehaviour
{
    [SerializeField] int amountOfSimultanousBubbles;
    [SerializeField] float bubbleDuration;
    [SerializeField] float minBubbleDelay;
    [SerializeField] float maxBubbleDelay;

    float nextBubblesTime;
    string filePath;
    Sprite[] emojies;
    SpriteStateManager[] possibleCandidates;

    void Start()
    {
        nextBubblesTime = Time.time + Random.Range(minBubbleDelay, maxBubbleDelay);
        //filePath = Path.Combine(Application.dataPath, "Resources", "emojis-x2-64x64");
        emojies = Resources.LoadAll<Sprite>("emojis-x2-64x64");
    }

    void Update()
    {
        possibleCandidates = SpawnManager.Instance.registeredSprites.Select(a => a.AssignedPrefab.GetComponent<SpriteStateManager>()).Where(a => !a.EmojiContainer.gameObject.activeInHierarchy).ToArray();
        if (Time.time >= nextBubblesTime && possibleCandidates.Count() >= amountOfSimultanousBubbles) 
        {
            for(int i = 0; i < amountOfSimultanousBubbles; i++)
            {
                SpriteStateManager sprite = possibleCandidates[Random.Range(0, possibleCandidates.Length)];
                if(sprite != null)
                {
                    sprite.SpeechBubble.gameObject.SetActive(true);
                    sprite.EmojiContainer.sprite = emojies[Random.Range(0, emojies.Length)];
                    sprite.Invoke(nameof(sprite.HideBubble), bubbleDuration);
                }
            }
            nextBubblesTime = Time.time + Random.Range(minBubbleDelay, maxBubbleDelay);
        }
    }
}
