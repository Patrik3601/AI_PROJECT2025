using UnityEngine;

public class ParentPlayerScript : MonoBehaviour
{
    public Motion motion;
    public FollowCam followCam;


    public void Died()
    {
        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15) // void
        {
            Died();
            GlobalGameManager._instance.PlayerDied();
        }
    }
}