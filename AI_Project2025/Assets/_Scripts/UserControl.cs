using System;
using System.Collections;
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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public EventHandler OnPlayersJoined;
    public EventHandler<PlayerSwitchEventArgs> OnPlayerSwitch;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) // SPAWN
        {
            Spawn();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // change to previous player
        {
            OnPlayerSwitch?.Invoke(this, new PlayerSwitchEventArgs(KeyCode.LeftArrow));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnPlayerSwitch?.Invoke(this, new PlayerSwitchEventArgs(KeyCode.RightArrow));
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
          var randomX = UnityEngine.Random.Range(-15f, 15f);

            var randomY = UnityEngine.Random.Range(-2f, 2f);
            Instantiate(objectToSpawn, new Vector3(spawnPosition.x + randomX, spawnPosition.y + randomY), Quaternion.identity);
            OnPlayersJoined?.Invoke(this, null);
        }
    }
}
