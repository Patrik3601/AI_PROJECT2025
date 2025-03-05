using Assets._Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public int ID = 0;

    public List<ParentPlayerScript> players;
    public void AddPlayer(ParentPlayerScript player)
    {
        players.Add(player);
        player.UID = ID++;
        player.OnPlayerDead += DeadPlayer;
    }

    private void DeadPlayer(object sender, PlayerDeadEventArgs e)
    {
        e.player.gameObject.SetActive(false);
        players.Remove(e.player);
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
