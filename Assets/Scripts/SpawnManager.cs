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
                bodyContainer.GetComponent<SpriteRenderer>().sprite = nextSprite.BodySprite;
                headContainer.GetComponent<SpriteRenderer>().sprite = nextSprite.HeadSprite;

                newPrefab.transform.localScale /= scaleFactor;
                newPrefab.GetComponent<SpriteStateManager>().data = nextSprite;

                waitingRoom.Remove(nextSprite);
                nextSprite.AssignedPrefab = newPrefab;
                registeredSprites.Add(nextSprite);
            }
        }
    }

    public void RefreshWaitingRoom()
    {
        List<SpriteDataContainer> newFiles = new List<SpriteDataContainer>();
        string[] directories = Directory.GetDirectories(Path.Combine(Application.dataPath, "Resources", filePath));
        foreach (string directory in directories)
        {
            // TODO: Naming Convention implementieren
            string[] files = Directory.GetFiles(directory);
            string pathToBody = files.FirstOrDefault(a => a.Split('\\').Last().StartsWith("Body"));
            string pathToHead = files.FirstOrDefault(a => a.Split('\\').Last().StartsWith("Eyes"));
            if (pathToBody != null && pathToHead != null)
            {
                pathToBody = Path.Combine(filePath, Path.GetFileNameWithoutExtension(directory), Path.GetFileNameWithoutExtension(pathToBody));
                pathToHead = Path.Combine(filePath, Path.GetFileNameWithoutExtension(directory), Path.GetFileNameWithoutExtension(pathToHead));

                var bodySprite = Resources.Load<Sprite>(pathToBody);
                var headSprite = Resources.Load<Sprite>(pathToHead);
                if(bodySprite != null && headSprite != null) 
                {
                    newFiles.Add(new SpriteDataContainer(directory, bodySprite, headSprite));
                }
            }
        }
        waitingRoom = newFiles.Where(newFile => !registeredSprites.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
    }
}
