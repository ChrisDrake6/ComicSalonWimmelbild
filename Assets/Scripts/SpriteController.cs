using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] float speed; 
    
    void Update()
    {
        gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, new Vector2(1000, 400), speed * Time.deltaTime);
    }
}
