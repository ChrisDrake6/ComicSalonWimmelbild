using UnityEngine;

[CreateAssetMenu(fileName = "RadialMenuElement", menuName = "RadialMenu/Element", order = 2)]
public class RadialMenuElementSO : ScriptableObject
{
    public PossibleActions Action;
    public Sprite Icon;
}
