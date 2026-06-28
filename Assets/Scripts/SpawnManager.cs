using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public List<FigureDataContainer> registeredFigures = new List<FigureDataContainer>();
    [SerializeField] string filePath;
    [SerializeField] GameObject figurePrefab;
    [SerializeField] float refreshInterval;
    [SerializeField] float spawnInterval;
    [SerializeField] Transform figureContainer;
    [SerializeField] float scaleFactor;
    [SerializeField] int maxFigureCount;

    float nextRefreshTime;
    float nextSpawnTime;
    List<FigureDataContainer> waitingRoom = new List<FigureDataContainer>();
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
        foreach (GameObject spawnPoint in spawnPoints)
        {
            spawnPoint.transform.parent.GetComponent<Animator>().SetBool("Opened", waitingRoom.Count != 0);
        }

        if (Time.time >= nextRefreshTime)
        {
            RefreshWaitingRoom();
            nextRefreshTime += refreshInterval;
        }
        if (Time.time >= nextSpawnTime)
        {
            FigureDataContainer nextFigure = waitingRoom.FirstOrDefault();
            if (nextFigure != null)
            {
                GameObject nextSpawnPoint = spawnPoints[spawnPointCycleTick % (spawnPoints.Length)];
                GameObject newPrefab = Instantiate(figurePrefab, nextSpawnPoint.transform);
                newPrefab.transform.parent = figureContainer;

                spawnPointCycleTick++;
                nextSpawnTime += spawnInterval;

                GameObject bodyContainer = newPrefab.transform.GetChild(0).gameObject;
                GameObject headContainer = newPrefab.transform.GetChild(1).gameObject;

                Texture2D bodyTex = new Texture2D(2, 2);
                Texture2D headTex = new Texture2D(2, 2);

                if (bodyTex.LoadImage(nextFigure.BodyTexData) && headTex.LoadImage(nextFigure.HeadTexData))
                {
                    Sprite bodySprite = Sprite.Create(bodyTex, new Rect(0, 0, bodyTex.width, bodyTex.height), new Vector2(0.5F, 0.5F), 100F);
                    Sprite headSprite = Sprite.Create(headTex, new Rect(0, 0, headTex.width, headTex.height), new Vector2(0.5F, 0.5F), 100F);

                    bodyContainer.GetComponent<SpriteRenderer>().sprite = headSprite;
                    headContainer.GetComponent<SpriteRenderer>().sprite = bodySprite;

                    nextFigure.BodySprite = bodySprite;
                    nextFigure.HeadSprite = headSprite;

                    newPrefab.transform.localScale /= scaleFactor;
                    newPrefab.GetComponent<PlayerFigureController>().FigureData = nextFigure;
                    newPrefab.name = "Figure" + Time.time;

                    nextFigure.AssignedPrefab = newPrefab;
                }
                else
                {
                    Destroy(bodyTex);
                    Destroy(headTex);
                    Destroy(newPrefab);
                    nextFigure.PresentOnScene = false;
                }
                waitingRoom.Remove(nextFigure);
                registeredFigures.Add(nextFigure);

                List<FigureDataContainer> presentSprites = registeredFigures.Where(a => a.PresentOnScene).ToList();
                if (presentSprites.Count > maxFigureCount)
                {
                    FigureDataContainer oldestSpriteData = presentSprites.FirstOrDefault();
                    if (oldestSpriteData != null)
                    {
                        PlayerFigureController oldestFigure = oldestSpriteData.AssignedPrefab.GetComponent<PlayerFigureController>();
                        oldestFigure.StartLeaving();
                    }
                }
            }
        }
    }

    public void RefreshWaitingRoom()
    {
        List<FigureDataContainer> newFiles = new List<FigureDataContainer>();
        string pathToDirectory = "";
        if (Application.isEditor)
        {
            pathToDirectory = Path.Combine(Application.dataPath, "Resources", filePath);
        }
        else
        {
            pathToDirectory = Path.Combine(Application.persistentDataPath, filePath);
        }
        if (!Directory.Exists(pathToDirectory))
        {
            Directory.CreateDirectory(pathToDirectory);
        }
        string[] directories = Directory.GetDirectories(pathToDirectory);
        foreach (string directory in directories)
        {
            // TODO: Naming Convention implementieren
            string[] files = Directory.GetFiles(directory);
            string pathToBody = files.FirstOrDefault(a => a.Split('\\', '/').Last().ToLower().StartsWith("body"));
            string pathToHead = files.FirstOrDefault(a => a.Split('\\', '/').Last().ToLower().StartsWith("head") || a.Split('\\', '/').Last().ToLower().StartsWith("eyes"));
            if (pathToBody != null && pathToHead != null)
            {
                pathToBody = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToBody);
                pathToHead = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToHead);

                //var bodySprite = Resources.Load<Sprite>(pathToBody);
                //var headSprite = Resources.Load<Sprite>(pathToHead);

                // TODO: Use UnityWebrequest?
                byte[] headFileData = File.ReadAllBytes(pathToHead);
                byte[] bodyFileData = File.ReadAllBytes(pathToBody);

                newFiles.Add(new FigureDataContainer(directory, bodyFileData, headFileData));
            }
        }
        waitingRoom = newFiles.Where(newFile => !registeredFigures.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
    }
}
