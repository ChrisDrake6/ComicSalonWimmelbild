using UnityEngine;

public class FigureDataContainer
{
    public string PathToDirectory { get; private set; }
    public Sprite BodySprite { get; set; }
    public Sprite HeadSprite { get; set; }
    public GameObject AssignedPrefab { get; set; }
    public bool PresentOnScene { get; set; }

    public FigureDataContainer(string pathToDirectory, Sprite bodySprite, Sprite headSprite)
    {
        PathToDirectory = pathToDirectory;
        BodySprite = bodySprite;
        HeadSprite = headSprite;
        PresentOnScene = true;
    }
}
