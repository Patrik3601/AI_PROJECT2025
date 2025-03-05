using System;
using UnityEngine;

public class UserControl : MonoBehaviour
{
    public int spawnCount;
    public GameObject objectToSpawn;
    public Vector3 spawnPosition;

    public static UserControl _instance;
    public static UserControl Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("NO USER CONTROL");
            }
            return _instance;
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) // SPAWN
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
          var randomX = UnityEngine.Random.Range(-2f, 2f);

            var randomY = UnityEngine.Random.Range(-2f, 2f);
            Instantiate(objectToSpawn, new Vector3(spawnPosition.x + randomX, spawnPosition.y + randomY), Quaternion.identity);
        }
    }
}
