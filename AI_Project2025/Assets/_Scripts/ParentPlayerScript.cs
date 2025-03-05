using Assets._Scripts;
using System;
using UnityEngine;

public class ParentPlayerScript : MonoBehaviour
{
    public Motion motion;
    public FollowCam followCam;

    public EventHandler<PlayerDeadEventArgs> OnPlayerDead;

    public int UID;
    private void Start()
    {
        GlobalGameManager._instance.AddPlayer(this);
    }

     void Died()
    {
        OnPlayerDead?.Invoke(this, new PlayerDeadEventArgs(this));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15) // void
        {
            Died();
        }
    }
}