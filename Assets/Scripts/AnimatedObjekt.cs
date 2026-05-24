using UnityEngine;

public class AnimatedObjekt : MonoBehaviour
{
    [SerializeField] private float maxMovingPause = 5;
    [SerializeField] private float minMovingPause = 2;

    private float _nextBlinkTime;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Time.time >  _nextBlinkTime)
        {
            animator.SetTrigger("Move");
            _nextBlinkTime = Time.time + Random.Range(minMovingPause, maxMovingPause);
        }
    }
}
