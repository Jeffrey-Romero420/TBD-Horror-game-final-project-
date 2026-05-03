using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnKeys();
    }

    void SpawnKeys()
    {
        if (keyPrefab == null)
        {
            Debug.LogError("Key Prefab is missing!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        int keysToSpawn = Mathf.Min(3, spawnPoints.Length);

        System.Collections.Generic.List<Transform> available =
    new System.Collections.Generic.List<Transform>(spawnPoints);

        for (int i = 0; i < keysToSpawn; i++)
        {
            int index = Random.Range(0, available.Count);

            Instantiate(keyPrefab, available[index].position, Quaternion.identity);

            available.RemoveAt(index);
        }
    }


    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];

            if (point == null)
            {
                Gizmos.color = Color.red; // ❌ missing reference
            }
            else
            {
                Gizmos.color = Color.green; // ✅ valid spawn point
            }

            Vector3 pos = point != null ? point.position : transform.position;
            Gizmos.DrawWireSphere(pos, 0.3f);

            #if UNITY_EDITOR
            UnityEditor.Handles.Label(pos + Vector3.up * 0.3f, i.ToString());
            #endif
        }
    }





}