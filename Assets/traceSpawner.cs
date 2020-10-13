using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traceSpawner : MonoBehaviour
{
    public Transform prefab;

    bool spawnTraces = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTraces)
            makeTrace();
    }

    void makeTrace()
    {
        Instantiate(prefab,this.transform.position, Quaternion.identity);
    }

    public void changeStateSpawnTraces()
    {
        spawnTraces = !spawnTraces;
    }
}
