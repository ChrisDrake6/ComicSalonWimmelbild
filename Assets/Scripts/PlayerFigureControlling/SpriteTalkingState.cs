using UnityEngine;

public class SpriteTalkingState : SpriteBaseState
{
    private Animator _animator;
    private float _talkingTime;
    private float _talkingDelay;
    private SpriteStateManager _stateManager;

    private float nextSpeakTime;
    private float endTime;

    public SpriteTalkingState(Animator animator, float talkingTime, float talkingDelay, SpriteStateManager stateManager)
    {
        _talkingTime = talkingTime;
        _talkingDelay = talkingDelay;
        _animator = animator;
        _stateManager = stateManager;
    }

    public override void EnterState()
    {
        _stateManager.agent.avoidancePriority = 99;
        nextSpeakTime = Time.time + Random.Range(0.1f, _talkingDelay);
        endTime = Time.time + _talkingTime;
    }

    public override void UpdateState()
    {
        if (nextSpeakTime < Time.time)
        {
            _animator.Play("DoubleJump_Quick");
            ConversationManager.Instance.ShowBubble(_stateManager);
            nextSpeakTime = Time.time + Random.Range(0.1f, _talkingDelay);
        }
        if (endTime < Time.time)
        {
            _stateManager.SwitchState(_stateManager.idleState);
        }
    }

    public override void LeaveState() { }

    public override void OnTriggerEnter(Collider2D collision) { }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(new Vector3(_stateManager.transform.position.x, _stateManager.transform.position.y + 0.75F, 0), 0.1F);
    }
}
