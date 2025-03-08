using NUnit.Framework;
using UnityEngine;
using UnityEngine.LightTransport;

public class Motion : MonoBehaviour
{
    public ParentPlayerScript parentScript;


    public Rigidbody2D rb;
    public float jumpHeight;
    public float rayDistance;
    public float moveSpeed;

    public float boxCastSize;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        IsGrounded();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            // f(x)=-(sqrt(h)*4 x-sqrt(h))^(2)+1+h+o
            if (IsGrounded())
            {
                Jump();
            }
        }
        Stretch();

        if (Input.GetKey(KeyCode.A))
        {
            if (!IsWallInFront(-1))
            {

                Move(-1);

            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!IsWallInFront(1))
            {
                Move(1);

            }

        }
    }

    private void Stretch()
    {

        float vel = rb.linearVelocityY;
        if (vel > 0)
        {

        }
        float x = vel / 30;
        float y = vel / 15;
        transform.localScale = new Vector3(Mathf.Abs(1.5f - Mathf.Clamp(x, 0.5f, 1f)), Mathf.Clamp(y, 1, 1.5f), transform.localScale.z);

    }

    public void Jump()
    {
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
        //Debug.Log(jumpForce);

        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }
    public void Move(float dir)
    {
        var d = dir;

        if (d > 0)
        {
            d = 1;
        }
        else if(d < 0)
        {
            d = -1;
        }
        //rb.position = new Vector3(transform.position.x + dir * moveSpeed * Time.deltaTime, transform.position.y);

        var moveto = new Vector3(transform.position.x + d * moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        //if (moveto.x >= maxLeftPos.x)
        {
            transform.position = moveto;
            //maxLeftPos = FollowCam._instance.cam.ViewportToWorldPoint(new Vector3(0, 0.5f, FollowCam._instance.cam.nearClipPlane));  
        }
    }

    public bool IsWallInFront(int dir)
    {
        var hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right * dir, boxCastSize/* 0.61f*/, 1 << 6);
        if (hit)
        {
            return true;
        }
        return false;
    }
  public  Transform[] rays;
    public bool IsGrounded()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            var hit = Physics2D.Raycast(rays[i].position, -rays[i].transform.up, rayDistance, 1 << 6);
            if (hit)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * transform.lossyScale.y / 2);
    }
}
