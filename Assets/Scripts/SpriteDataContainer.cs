using UnityEngine;

public class SpriteDataContainer
{
    public string PathToDirectory { get; private set; }
    public Texture2D BodyTex { get; private set; }
    public Texture2D HeadTex { get; private set; }
    public Sprite BodySprite { get; set; }
    public Sprite HeadSprite { get; set; }
    public GameObject AssignedPrefab { get; set; }
    public bool PresentOnScene { get; set; }

    public SpriteDataContainer(string pathToDirectory, Texture2D bodyTex, Texture2D headTex)
    {
        PathToDirectory = pathToDirectory;
        BodyTex = bodyTex;
        HeadTex = headTex;
        PresentOnScene = true;
    }
}
