using System;
using System.Linq;
using UnityEngine;

public class HatMaker : MonoBehaviour
{
    [SerializeField] private GameObject hatPrefab;
    [SerializeField] private GameObject creationPoint;
    [SerializeField] private float creationTime;
    [SerializeField] private int maxHatAmount;

    int _hatCount = 0;

    void Start()
    {
        HatGuyWavingDone.CreationFinished += CreateHat;
        Hat.HatDestroyed += DecreaseHatCount;
        InvokeRepeating("StartCreating", creationTime, creationTime);
    }

    public void StartCreating()
    {
        if (maxHatAmount > _hatCount && !FindObjectsByType<Hat>().Any(a => a.IsClaimed == false))
        {
            GetComponent<Animator>().SetTrigger("StartCreating");
        }
    }

    private void OnDestroy()
    {
        HatGuyWavingDone.CreationFinished -= CreateHat;
        Hat.HatDestroyed -= DecreaseHatCount;
    }

    public void CreateHat()
    {
        _hatCount++;
        Instantiate(hatPrefab, creationPoint.transform.position, hatPrefab.transform.rotation);
    }

    private void DecreaseHatCount()
    {
        _hatCount--;
    }
}
