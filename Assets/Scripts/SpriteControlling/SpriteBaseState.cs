using UnityEngine;

public abstract class SpriteBaseState
{
    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void LeaveState();


    public abstract void OnTriggerEnter(Collider2D collision);

    public abstract void OnDrawGizmos();
}
