using UnityEngine;

public class SpriteLeavingState : SpriteBaseState
{
    public override void EnterState(SpriteStateManager sprite)
    {
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
    }

    public override void OnDrawGizmos(SpriteStateManager sprite)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(sprite.transform.position.x, sprite.transform.position.y + 0.75F, 0), 0.1F);
    }
}
