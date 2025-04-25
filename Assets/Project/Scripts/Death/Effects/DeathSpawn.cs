using System.Collections;
using UnityEngine;

[AddComponentMenu("Project/Death/Death Spawn")]
public class DeathSpawn : MonoBehaviour, IDeathCallback
{
    [SerializeField]
    private GameObject spawnPrefab;
    [SerializeField, Min(0f)]
    private float waitDuration = 0f;

    public IEnumerator OnDeathCallback()
    {
        if (spawnPrefab == null) yield break;
        if (waitDuration > 0f) yield return new WaitForSeconds(waitDuration);
        Instantiate(spawnPrefab, transform.position, transform.rotation);
    }
}