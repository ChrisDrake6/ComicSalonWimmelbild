using System;
using System.Linq;
using UnityEngine;

public class Hat : MonoBehaviour
{    
    [SerializeField] private float lifeTime;
    [SerializeField] private float claimRequestInterval;
    [SerializeField] private HatData[] possibleHats;
    [SerializeField] private GameObject poofPrefab;

    public bool IsClaimed { get; set; }

    public static event Action HatDestroyed;

    private HatData _chosenHat;
    private float _nextClaimRequestTime = 0;
    private int _requestCount = 0;
    private bool _pickedUp;

    void Start()
    {
        Instantiate(poofPrefab,transform.position, poofPrefab.transform.rotation);
        _chosenHat = possibleHats[UnityEngine.Random.Range(0, possibleHats.Length)];
        GetComponent<SpriteRenderer>().sprite = _chosenHat.Sprite;
        Invoke("SelfDestruct", lifeTime);
    }

    private void Update()
    {
        if (!_pickedUp && Time.time > _nextClaimRequestTime)
        {
            _requestCount++;
            if (_requestCount == 3)
            {
                SelfDestruct();
            }
            if (!IsClaimed)
            {
                SpriteDataContainer[] possibleCarriers = SpawnManager.Instance.registeredSprites.Where(a =>
                {
                    SpriteStateManager sprite = a.AssignedPrefab.GetComponent<SpriteStateManager>();
                    return (sprite.CurrentHat == null &&
                    !sprite.IsInGroup &&
                    sprite.currentState != sprite.talkingState &&
                    sprite.currentState != sprite.leavingState &&
                    sprite.currentState != sprite.getHatState);
                }).ToArray();
                possibleCarriers[UnityEngine.Random.Range(0, possibleCarriers.Length)].AssignedPrefab.GetComponent<SpriteStateManager>().OnNewHatCreated(gameObject);
                _nextClaimRequestTime = Time.time + claimRequestInterval;
                IsClaimed = true;
            }
        }
    }

    public void SelfDestruct()
    {
        GameObject poof = Instantiate(poofPrefab,transform.position, poofPrefab.transform.rotation);
        poof.transform.localScale = Vector3.one * (_chosenHat.scale) / 50;
        HatDestroyed.Invoke();
        Destroy(gameObject);
    }

    public void OnPickUp()
    {
        _pickedUp = true;
        transform.localPosition = new Vector2(0, _chosenHat.YOffset);
        transform.localScale = Vector3.one * (_chosenHat.scale);
    }
}
