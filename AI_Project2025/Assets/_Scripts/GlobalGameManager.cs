using Assets._Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager _instance;
    public static GlobalGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("NO GAME MANAGER");
            }
            return _instance;
        }
    }
    public int IDCounter = 0;
    public int currentPlayerIndex = 0;

    
    
    private void Start()
    {
        Debug.Log("START");
        Academy.Instance.AutomaticSteppingEnabled = false;
    }
    private void FixedUpdate()
    {
        Debug.Log("FIXED UPDATE");
        Academy.Instance.EnvironmentStep();
    }
    
    
    
    
    
    public List<ParentPlayerScript> players;
    public void AddPlayer(ParentPlayerScript player)
    {
        players.Add(player);
        player.UID = IDCounter++;
        player.OnPlayerDead += DeadPlayer;
    }

    private void DeadPlayer(object sender, PlayerDeadEventArgs e)
    {
        e.player.gameObject.SetActive(false);
        players.Remove(e.player);


        if (players.Count > 0)
        {
            currentPlayerIndex = players.Count - 1;
        }
        else
        {
            currentPlayerIndex = 0;
        }
    }
    public int SetCurrentPlayerIndex(int newValue) // 1 | -1
    {
        if (newValue > 0)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }
        else
        {
            currentPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count;
        }
        return currentPlayerIndex;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            players = new();
        }
        else
        {
            Destroy(this);
        }
    }
}
