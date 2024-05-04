using UnityEngine;

public class SpriteDataContainer
{
    public string PathToDirectory { get; private set; }
    public Sprite BodySprite { get; private set; }
    public Sprite HeadSprite { get; private set; }
    public GameObject AssignedPrefab { get; set; }

    public SpriteDataContainer(string pathToDirectory, Sprite bodySprite, Sprite headSprite)
    {
        PathToDirectory = pathToDirectory;        
        BodySprite = bodySprite;
        HeadSprite = headSprite;
    }
}
