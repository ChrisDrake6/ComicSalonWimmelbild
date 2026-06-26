using UnityEngine;
using UnityEngine.AI;

public class PlayerFigureController : MonoBehaviour
{
    public SpriteRenderer EmojiContainer;
    public GameObject SpeechBubble;

    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.avoidancePriority = Random.Range(0, 99);
    }

    public void HideBubble()
    {
        SpeechBubble.gameObject.SetActive(false);
    }

    public void Despawn()
    {
        SpriteRenderer bodyRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer headRenderer = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        Sprite bodySprite = bodyRenderer.sprite;
        Sprite headSprite = headRenderer.sprite;

        Texture2D bodyTex = bodySprite.texture;
        Texture2D headTex = headSprite.texture;

        //if (CurrentHat != null)
        //{
        //    CurrentHat?.GetComponent<Hat>().SelfDestruct();
        //}

        Destroy(bodyTex);
        Destroy(headTex);
        Destroy(bodySprite);
        Destroy(headSprite);
        Destroy(gameObject);
    }
}
