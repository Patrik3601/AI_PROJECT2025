using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public ParentPlayerScript parent;


    public Camera cam;
    public GameObject toFollow;
    public float followSpeed;

    public bool enableFollow;
    public float maxLeftX;
    private void Update()
    {
        if (enableFollow)
        {
            var toPos = Vector3.Lerp(cam.transform.position, new Vector3(toFollow.transform.position.x, toFollow.transform.position.y, -10), followSpeed * Time.deltaTime);
            if (toPos.x > maxLeftX)
            {
              
                cam.transform.position = toPos;

                maxLeftX = cam.transform.position.x;
            }
            else
            {
                toPos = Vector3.Lerp(cam.transform.position, new Vector3(cam.transform.position.x, toFollow.transform.position.y, -10), followSpeed * Time.deltaTime);
                cam.transform.position = toPos;

            }
        }
    }
}