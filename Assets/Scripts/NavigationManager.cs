using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private float roamingRadius;

    public static NavigationManager Instance { get; private set; }

    public NavigationManager()
    {
        Instance = this;
    }

    public Vector3 GetRandomLongDistanceDestination(Transform origin)
    {
        Vector3 randomPoint = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 10);
        Vector3 randomDirection = Camera.main.ViewportToWorldPoint(randomPoint);
        randomDirection.z = 0;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, Camera.main.pixelWidth, 1);
        return hit.position;
    }

    public Vector3 GetRandomShortDistanceDestination(Transform origin)
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
        randomDirection += origin.position;
        randomDirection.z = 0;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, Camera.main.pixelWidth, 1);
        return hit.position;
    }

    public Vector3 GetClosestSpawnPosition(Transform origin)
    {
        Transform closestSpawnPoint = SpawnManager.Instance.GetSpawnPoints().OrderBy(a => Vector3.Distance(origin.position, a.position)).First();
        return closestSpawnPoint.position;
    }
}
