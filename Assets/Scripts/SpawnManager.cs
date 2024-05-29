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

                Texture2D bodyTex = new Texture2D(2, 2);
                Texture2D headTex = new Texture2D(2, 2);

                if (bodyTex.LoadImage(nextSprite.BodyTexData) && headTex.LoadImage(nextSprite.HeadTexData))
                {
                    Sprite bodySprite = Sprite.Create(bodyTex, new Rect(0, 0, bodyTex.width, bodyTex.height), new Vector2(0.5F, 0.5F), 100F);
                    Sprite headSprite = Sprite.Create(headTex, new Rect(0, 0, headTex.width, headTex.height), new Vector2(0.5F, 0.5F), 100F);

                    bodyContainer.GetComponent<SpriteRenderer>().sprite = headSprite;
                    headContainer.GetComponent<SpriteRenderer>().sprite = bodySprite;

                    nextSprite.BodySprite = bodySprite;
                    nextSprite.HeadSprite = headSprite;

                    newPrefab.transform.localScale /= scaleFactor;
                    newPrefab.GetComponent<SpriteStateManager>().data = nextSprite;

                    nextSprite.AssignedPrefab = newPrefab;
                }
                else
                {
                    Destroy(newPrefab);
                    nextSprite.PresentOnScene = false;
                }
                waitingRoom.Remove(nextSprite);
                registeredSprites.Add(nextSprite);

                List<SpriteDataContainer> presentSprites = registeredSprites.Where(a => a.PresentOnScene).ToList();
                if (presentSprites.Count > maxSpriteCount)
                {
                    SpriteDataContainer oldestSpriteData = presentSprites.FirstOrDefault();
                    if (oldestSpriteData != null)
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

                newFiles.Add(new SpriteDataContainer(directory, bodyFileData, headFileData));
            }
        }
        waitingRoom = newFiles.Where(newFile => !registeredSprites.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
    }
}
