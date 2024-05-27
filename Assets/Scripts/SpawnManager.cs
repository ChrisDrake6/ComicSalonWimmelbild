using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public List<SpriteDataContainer> registeredSprites = new List<SpriteDataContainer>();
    [SerializeField] string filePath;
    [SerializeField] GameObject spritePrefab;
    [SerializeField] float refreshInterval;
    [SerializeField] float spawnInterval;
    [SerializeField] Transform spriteContainer;
    [SerializeField] float scaleFactor;
    [SerializeField] int maxSpriteCount;

    float nextRefreshTime;
    float nextSpawnTime;
    List<SpriteDataContainer> waitingRoom = new List<SpriteDataContainer>();
    int spawnPointCycleTick = 0;

    public static SpawnManager Instance { get; private set; }

    public SpawnManager()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshWaitingRoom();
        nextRefreshTime = Time.time + refreshInterval;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Remove SpawnPointsEntries if empty
        spawnPoints = spawnPoints.Where(a => a != null).ToArray();

        if (Time.time >= nextRefreshTime)
        {
            RefreshWaitingRoom();
            nextRefreshTime += refreshInterval;
        }
        if (Time.time >= nextSpawnTime)
        {
            SpriteDataContainer nextSprite = waitingRoom.FirstOrDefault();
            if (nextSprite != null)
            {
                GameObject nextSpawnPoint = spawnPoints[spawnPointCycleTick % (spawnPoints.Length)];
                GameObject newPrefab = Instantiate(spritePrefab, nextSpawnPoint.transform);
                newPrefab.transform.parent = spriteContainer;

                spawnPointCycleTick++;
                nextSpawnTime += spawnInterval;

                GameObject bodyContainer = newPrefab.transform.GetChild(0).gameObject;
                GameObject headContainer = newPrefab.transform.GetChild(1).gameObject;

                Sprite headSprite = Sprite.Create(nextSprite.HeadTex, new Rect(0, 0, nextSprite.HeadTex.width, nextSprite.HeadTex.height), new Vector2(0, 0), 100F);
                Sprite bodySprite = Sprite.Create(nextSprite.BodyTex, new Rect(0, 0, nextSprite.BodyTex.width, nextSprite.BodyTex.height), new Vector2(0, 0), 100F);

                bodyContainer.GetComponent<SpriteRenderer>().sprite = headSprite;
                headContainer.GetComponent<SpriteRenderer>().sprite = bodySprite;

                nextSprite.BodySprite = bodySprite;
                nextSprite.HeadSprite = headSprite;

                newPrefab.transform.localScale /= scaleFactor;
                newPrefab.GetComponent<SpriteStateManager>().data = nextSprite;

                waitingRoom.Remove(nextSprite);
                nextSprite.AssignedPrefab = newPrefab;
                registeredSprites.Add(nextSprite);

                List<SpriteDataContainer> presentSprites = registeredSprites.Where(a => a.PresentOnScene).ToList();
                if (presentSprites.Count > maxSpriteCount)
                {
                    SpriteDataContainer oldestSpriteData = presentSprites.FirstOrDefault();
                    if(oldestSpriteData != null)
                    {
                        SpriteStateManager oldestSprite = oldestSpriteData.AssignedPrefab.GetComponent<SpriteStateManager>();
                        oldestSprite.SwitchState(oldestSprite.leavingState);
                    }
                }
            }
        }
    }

    public void RefreshWaitingRoom()
    {
        List<SpriteDataContainer> newFiles = new List<SpriteDataContainer>();
        string pathToDirectory = Path.Combine(Application.dataPath, "Resources", filePath);
        if (!Directory.Exists(pathToDirectory))
        {
            Directory.CreateDirectory(pathToDirectory);
        }
        string[] directories = Directory.GetDirectories(pathToDirectory);
        foreach (string directory in directories)
        {
            // TODO: Naming Convention implementieren
            string[] files = Directory.GetFiles(directory);
            string pathToBody = files.FirstOrDefault(a => a.Split('\\').Last().ToLower().StartsWith("body"));
            string pathToHead = files.FirstOrDefault(a => a.Split('\\').Last().ToLower().StartsWith("head") || a.Split('\\').Last().ToLower().StartsWith("eyes"));
            if (pathToBody != null && pathToHead != null)
            {
                pathToBody = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToBody);
                pathToHead = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToHead);

                //var bodySprite = Resources.Load<Sprite>(pathToBody);
                //var headSprite = Resources.Load<Sprite>(pathToHead);

                byte[] headFileData = File.ReadAllBytes(pathToHead);
                byte[] bodyFileData = File.ReadAllBytes(pathToBody);

                Texture2D headTex = new Texture2D(2, 2);
                Texture2D bodyTex = new Texture2D(2, 2);

                if (headTex.LoadImage(headFileData) && bodyTex.LoadImage(bodyFileData))
                {
                    newFiles.Add(new SpriteDataContainer(directory, bodyTex, headTex));
                }
            }
        }
        waitingRoom = newFiles.Where(newFile => !registeredSprites.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
    }
}
