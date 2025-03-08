using System;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI _instance;
    public static PlayerUI Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("NO PLAYER UI");
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
    public TextMeshProUGUI playerID_UI;
    public TextMeshProUGUI playerGen_UI;

    private void Start()
    {
        UserControl._instance.OnPlayersJoined += PlayerJoined;
        UserControl._instance.OnPlayerSwitch += PlayerSwitch;
        for (int i = 0; i < GlobalGameManager._instance.players.Count; i++)
        {
            GlobalGameManager._instance.players[i].PlayerAgent.OnNextGeneration += NextGen;
            
        }
    }

    private void NextGen(object sender, EventArgs e)
    {
        GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex].currentGeneration++;
       
        UpdateUI();
    }

    private void PlayerSwitch(object sender, PlayerSwitchEventArgs e)
    {
        switch (e.arrow)
        {
            case KeyCode.LeftArrow:
                GlobalGameManager._instance.SetCurrentPlayerIndex(-1);
                break;
            case KeyCode.RightArrow:
                GlobalGameManager._instance.SetCurrentPlayerIndex(1);
                break;
            default:
                break;
        }
        UpdateUI();
    }
    private void PlayerJoined(object sender, EventArgs e)
    {
        UpdateUI();
        
    }
    void UpdateUI()
    {
        playerID_UI.text = $"Player: {GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex].UID}";
        playerGen_UI.text = $"Generation: {GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex].currentGeneration}";
    }
}