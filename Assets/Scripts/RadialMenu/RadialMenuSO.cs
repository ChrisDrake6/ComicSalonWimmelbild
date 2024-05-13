using UnityEngine;

[CreateAssetMenu(fileName = "RadialMenu", menuName = "RadialMenu/Ring", order = 1)]
public class RadialMenuSO : ScriptableObject
{
    public RadialMenuElementSO[] Elements;
}
