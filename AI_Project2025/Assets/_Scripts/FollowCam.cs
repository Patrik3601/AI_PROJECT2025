using Assets._Scripts;
using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FollowCam : MonoBehaviour
{
    public ParentPlayerScript parent;

    public static FollowCam _instance;
    public static FollowCam Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("NO CAMERA");
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

    public Camera cam;
    public GameObject toFollow;
    public float followSpeed;

    public bool enableFollow;

    private void Start()
    {
        UserControl._instance.OnPlayersJoined += PlayerJoined;
        UserControl._instance.OnPlayerSwitch += PlayerSwitch;
    }
    private void PlayerJoined(object sender, EventArgs e)
    {
        SetToFollow(GlobalGameManager._instance.players[0]);
        GlobalGameManager._instance.players[GlobalGameManager._instance.players.Count - 1].OnPlayerDead += PlayerDied;
    }

    private void PlayerDied(object sender, PlayerDeadEventArgs e)
    {
        if (GlobalGameManager._instance.players.Count > 0)
        {
            SetToFollow(GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex]);

        }
        GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex].OnPlayerDead -= PlayerDied;
    }

    public void SetToFollow(ParentPlayerScript player)
    {
        toFollow = player.gameObject;
    }
    private void PlayerSwitch(object sender, PlayerSwitchEventArgs e)
    {
        if (GlobalGameManager._instance.players.Count >= 0)
        {
            SetToFollow(GlobalGameManager._instance.players[GlobalGameManager._instance.currentPlayerIndex]);
        }
    }


    private void Update()
    {
        if (enableFollow)
        {
            var toPos = Vector3.Lerp(cam.transform.position, new Vector3(toFollow.transform.position.x, toFollow.transform.position.y, -10), followSpeed * Time.deltaTime);
            cam.transform.position = toPos;
        }
    }
}