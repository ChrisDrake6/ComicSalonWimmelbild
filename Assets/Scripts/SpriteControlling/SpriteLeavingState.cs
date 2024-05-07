using UnityEngine;

public class SpriteLeavingState : SpriteBaseState
{
    public override void EnterState(SpriteStateManager sprite)
    {
        if (sprite.debugStatusDisplay.enabled)
        {
            sprite.debugStatusDisplay.color = Color.red;
        }
    }

    public override void UpdateState(SpriteStateManager sprite)
    {
    }
}
