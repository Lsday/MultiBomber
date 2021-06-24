using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoolTester : MonoBehaviour
{

    PrefabPoolManager prefabPoolManager;

    void Start()
    {
        //prefabPoolManager = FindObjectOfType<PrefabPoolManager>();
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(300,300,100,25),"ADD BOMB"))
        {
            // Set up bullet on server
            GameObject bullet = prefabPoolManager.GetFromPool(transform.position + Random.insideUnitSphere*2, Quaternion.identity);

            // tell server to send SpawnMessage, which will call SpawnHandler on client
            NetworkServer.Spawn(bullet);
        }
    }
}
