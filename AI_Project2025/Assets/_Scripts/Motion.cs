using NUnit.Framework;
using System.Collections;
using Unity.VisualScripting;
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
    public Transform[] checkHoleRays;
    public bool isGrounded;
    public bool isHoleLeft;
    public bool isHoleRight;
    public bool isWallLeft;
    public bool isWallRight;
    [Header("DEBUG")]
    public bool SHOWISGROUNDED = false;
    public bool SHOWHOLERIGHT = false;
    public bool SHOWHOLELEFT = false;
    public bool SHOWVELOCITYX = false;
    public bool SHOWVELOCITYY = false;
    public bool SHOW_WALL_RIGHT = false;
    public bool SHOW_WALL_LEFT = false;
    public bool SHOW_IS_STUCK = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;

    }
    public float xSpeed;
    Vector3 lastPosition;
    Vector3 lastPositionIsStuck;
    public bool isStuck;
    public float stuckTimer = 0f;
    public float stuckThresholdTime = 0.5f;
    private void Update()
    {
        IsStuck();
        IsGrounded();
        IsThereHoleLeft();
        IsThereHoleRight();
        IsWallLeft();
        IsWallRight();
        if (SHOWISGROUNDED) Debug.Log("Isgrounded " + isGrounded);
        if (SHOWHOLELEFT) Debug.Log("Hole Left " + isHoleLeft);
        if (SHOWHOLERIGHT) Debug.Log("Hole Right " + isHoleRight);
        if (SHOWVELOCITYX)
        {
            xSpeed = Mathf.Abs(transform.position.x - lastPosition.x) / Time.smoothDeltaTime;
            lastPosition = transform.position;
            Debug.Log("Velocity X " + xSpeed);
        }
        if (SHOWVELOCITYY)
        {
            Debug.Log("Velocity Y " + rb.linearVelocityY);
        }
        if (SHOW_WALL_RIGHT) Debug.Log("Wall Right " + isWallRight);
        if (SHOW_WALL_LEFT) Debug.Log("Wall Left " + isWallLeft);
        if (SHOW_IS_STUCK)
        {
            Debug.Log("Is Stuck " + isStuck);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            // f(x)=-(sqrt(h)*4 x-sqrt(h))^(2)+1+h+o
            if (isGrounded)
            {
                Jump();
            }
        }
        //Stretch();

        if (Input.GetKey(KeyCode.A))
        {
            if (!isWallLeft)
            {

                Move(-1);

            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!isWallRight)
            {
                Move(1);

            }

        }
    }

    private void IsStuck()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThresholdTime)
            {
                isStuck = true;
            }
        }
        else
        {
            isStuck = false;
            stuckTimer = 0;
        }
        lastPosition = transform.position;
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

    public bool canJump = true;
    public void Jump()
    {
        if (canJump)
        {
            StartCoroutine(Jumpc());
        }
    }
    IEnumerator Jumpc()
    {

        canJump = false;
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        canJump = true;

    }
    public void Move(float dir)
    {

        //rb.position = new Vector3(transform.position.x + dir * moveSpeed * Time.deltaTime, transform.position.y);

        var moveto = new Vector3(transform.position.x + dir * moveSpeed * Time.smoothDeltaTime, transform.position.y, transform.position.z);


        transform.position = moveto;

    }

    public void IsWallLeft()
    {
        var hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right * -1, boxCastSize/* 0.61f*/, 1 << 6);
        if (hit)
        {
            isWallLeft = true;
            return;
        }
        isWallLeft = false;
    }
    public void IsWallRight()
    {
        var hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right, boxCastSize/* 0.61f*/, 1 << 6);
        if (hit)
        {
            isWallRight= true;
            return;
        }
        isWallRight= false;
    }

    public Transform[] rays;


    public void IsGrounded()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            var hit = Physics2D.Raycast(rays[i].position, -rays[i].transform.up, rayDistance, 1 << 6);
            if (hit)
            {

                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }
    public void IsThereHoleLeft()
    {
        var hit = Physics2D.Raycast(checkHoleRays[0].position, -checkHoleRays[0].transform.up, Mathf.Infinity, 1 << 6);
        if (!hit)
        {
            isHoleLeft = true;
            return;
        }
        isHoleLeft = false;
    }
    public void IsThereHoleRight()
    {
        var hit = Physics2D.Raycast(checkHoleRays[1].position, -checkHoleRays[1].transform.up, Mathf.Infinity, 1 << 6);
        if (!hit)
        {
            isHoleRight = true;
            return;
        }
        isHoleRight = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * transform.lossyScale.y / 2);
    }
}
