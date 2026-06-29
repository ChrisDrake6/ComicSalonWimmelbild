using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private string filePath;
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private float refreshInterval;
    [SerializeField] private float spawnInterval;
    [SerializeField] private Transform figureContainer;
    [SerializeField] private float scaleFactor;
    [SerializeField] private int maxFigureCount;

    private float _nextRefreshTime;
    private float _nextSpawnTime;
    private List<FigureDataContainer> _waitingRoom = new List<FigureDataContainer>();
    private int _spawnPointCycleTick = 0;

    public List<FigureDataContainer> RegisteredFigures { get; set; } = new List<FigureDataContainer>();
    public static SpawnManager Instance { get; private set; }

    public SpawnManager()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshWaitingRoom();
        _nextRefreshTime = Time.time + refreshInterval;
        _nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Remove SpawnPointsEntries if empty
        spawnPoints = spawnPoints.Where(a => a != null).ToArray();
        foreach (Transform spawnPoint in spawnPoints)
        {
            spawnPoint.parent.GetComponent<Animator>().SetBool("Opened", _waitingRoom.Count != 0);
        }

        if (Time.time >= _nextRefreshTime)
        {
            RefreshWaitingRoom();
            _nextRefreshTime += refreshInterval;
        }
        if (Time.time >= _nextSpawnTime)
        {
            FigureDataContainer nextFigure = _waitingRoom.FirstOrDefault();
            if (nextFigure != null)
            {
                Transform nextSpawnPoint = spawnPoints[_spawnPointCycleTick % (spawnPoints.Length)];
                GameObject newPrefab = Instantiate(figurePrefab, nextSpawnPoint);
                newPrefab.transform.parent = figureContainer;

                _spawnPointCycleTick++;
                _nextSpawnTime += spawnInterval;

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
                _waitingRoom.Remove(nextFigure);
                RegisteredFigures.Add(nextFigure);

                List<FigureDataContainer> presentSprites = RegisteredFigures.Where(a => a.PresentOnScene).ToList();
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
        _waitingRoom = newFiles.Where(newFile => !RegisteredFigures.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
    }

    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }
}
