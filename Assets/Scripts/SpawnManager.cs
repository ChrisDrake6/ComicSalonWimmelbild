using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

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
    private List<string> _alreadyUsedDirectorys = new List<string>();
    private bool _isLoading;

    public List<FigureDataContainer> RegisteredFigures { get; set; } = new List<FigureDataContainer>();
    public static SpawnManager Instance { get; private set; }

    public SpawnManager()
    {
        Instance = this;
    }

    void Start()
    {
        ToggleRefresh();
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
            ToggleRefresh();
            _nextRefreshTime += refreshInterval;
        }
        if (Time.time >= _nextSpawnTime && _waitingRoom.Count > 0)
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

                bodyContainer.GetComponent<SpriteRenderer>().sprite = nextFigure.HeadSprite;
                headContainer.GetComponent<SpriteRenderer>().sprite = nextFigure.BodySprite;

                newPrefab.transform.localScale /= scaleFactor;
                newPrefab.GetComponent<PlayerFigureController>().FigureData = nextFigure;
                newPrefab.name = "Figure" + Time.time;

                nextFigure.AssignedPrefab = newPrefab;
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

    public void ToggleRefresh()
    {
        if (_waitingRoom.Count == 0 && !_isLoading)
        {
            _isLoading = true;
            StartCoroutine(RefreshWaitingRoom());
        }
    }

    private IEnumerator RefreshWaitingRoom()
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
            if (_alreadyUsedDirectorys.Contains(directory))
            {
                break;
            }

            _alreadyUsedDirectorys.Add(directory);

            // TODO: Naming Convention implementieren
            string[] files = Directory.GetFiles(directory);
            string pathToBody = files.FirstOrDefault(a => a.Split('\\', '/').Last().ToLower().StartsWith("body"));
            string pathToHead = files.FirstOrDefault(a => a.Split('\\', '/').Last().ToLower().StartsWith("head") || a.Split('\\', '/').Last().ToLower().StartsWith("eyes"));
            if (pathToBody != null && pathToHead != null)
            {
                pathToBody = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToBody);
                pathToHead = Path.Combine(pathToDirectory, Path.GetFileNameWithoutExtension(directory), pathToHead);

                Task<byte[]> bodyTask = Task.Run(() => File.ReadAllBytesAsync(pathToBody));
                Task<byte[]> headTask = Task.Run(() => File.ReadAllBytesAsync(pathToHead));
                while (!bodyTask.IsCompleted || !headTask.IsCompleted)
                {
                    yield return null;
                }

                Texture2D bodyTex = new Texture2D(2, 2);
                Texture2D headTex = new Texture2D(2, 2);

                //https://discussions.unity.com/t/load-image-faster/786980 No way to make this async?
                if (bodyTex.LoadImage(bodyTask.Result) && headTex.LoadImage(headTask.Result))
                {
                    Sprite bodySprite = Sprite.Create(bodyTex, new Rect(0, 0, bodyTex.width, bodyTex.height), new Vector2(0.5F, 0.5F), 100F);
                    Sprite headSprite = Sprite.Create(headTex, new Rect(0, 0, headTex.width, headTex.height), new Vector2(0.5F, 0.5F), 100F);

                    newFiles.Add(new FigureDataContainer(directory, bodySprite, headSprite));
                }
                else
                {
                    Destroy(bodyTex);
                    Destroy(headTex);
                }
            }
        }
        _waitingRoom = newFiles.Where(newFile => !RegisteredFigures.Any(rS => rS.PathToDirectory == newFile.PathToDirectory)).ToList();
        _isLoading = false;
        yield return new WaitForEndOfFrame();
    }

    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }
}
