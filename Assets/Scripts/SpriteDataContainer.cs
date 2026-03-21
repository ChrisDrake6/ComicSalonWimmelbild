using UnityEngine;

public class SpriteDataContainer
{
    public string PathToDirectory { get; private set; }
    public byte[] BodyTexData { get; private set; }
    public byte[] HeadTexData { get; private set; }
    public Sprite BodySprite { get; set; }
    public Sprite HeadSprite { get; set; }
    public GameObject AssignedPrefab { get; set; }
    public bool PresentOnScene { get; set; }

    public SpriteDataContainer(string pathToDirectory, byte[] bodyTexData, byte[] headTexData)
    {
        PathToDirectory = pathToDirectory;
        BodyTexData = bodyTexData;
        HeadTexData = headTexData;
        PresentOnScene = true;
    }
}
