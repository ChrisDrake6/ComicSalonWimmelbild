public abstract class SpriteBaseState
{
    public abstract void EnterState(SpriteStateManager sprite);

    public abstract void UpdateState(SpriteStateManager sprite);

    public abstract void OnDrawGizmos(SpriteStateManager sprite);
}
