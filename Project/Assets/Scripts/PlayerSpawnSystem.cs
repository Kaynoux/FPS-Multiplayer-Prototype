using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
public class PlayerSpawnSystem : NetworkBehaviour
{
    public GameObject playerPrefab;
    private static List<Transform> spawnPoints = new List<Transform>();
    private int spawnPointIndex;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);
    
    
       
    


}
