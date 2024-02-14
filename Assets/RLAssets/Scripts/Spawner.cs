using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    Transform[] spawnPos = new Transform[6];
    [SerializeField]
    GameObject spawnObject; 
    // Start is called before the first frame update
    public void Spawn()
    {
        Instantiate(spawnObject, spawnPos[0].position, Quaternion.identity);
        Instantiate(spawnObject, spawnPos[1].position, Quaternion.identity);
        Instantiate(spawnObject, spawnPos[2].position, Quaternion.identity);

        Instantiate(spawnObject, spawnPos[3].position, Quaternion.identity);
        Instantiate(spawnObject, spawnPos[4].position, Quaternion.identity);
        Instantiate(spawnObject, spawnPos[5].position, Quaternion.identity);
    }
}
