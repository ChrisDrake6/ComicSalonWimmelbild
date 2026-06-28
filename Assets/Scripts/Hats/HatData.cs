using UnityEngine;

[CreateAssetMenu(fileName = "HatData", menuName = "Scriptable Objects/HatData")]
public class HatData : ScriptableObject
{
    public Sprite Sprite;
    public float YOffset;
    public float scale = 1;
}
