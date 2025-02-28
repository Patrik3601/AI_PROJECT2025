using System.Collections;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;

public class Motion : MonoBehaviour
{
    public float gravity = 9.81f;

    public bool isGrounded;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // f(x)=-(4x-2)²+4

            StartCoroutine(JumpCor());


        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - 5 * Time.deltaTime, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + 5 * Time.deltaTime, transform.position.y, transform.position.z);
        }
        Gravity();


    }

    public float amplitude;

    IEnumerator JumpCor()
    {
        float timer = 0;

        var x = transform.position.y;
        while (timer <= 1)
        {
            float jump = -Mathf.Pow(4 * timer - 1, 2);

            timer += Time.deltaTime;
            //transform.position = new Vector3(transform.position.x, transform.position.y + (50 * jump) * Time.deltaTime, transform.position.z);
            transform.position = new Vector3(transform.position.x, amplitude * Mathf.Sin(3.14f * timer) + x * Time.deltaTime, transform.position.z);
            //if (isGrounded)
            //{
            //    if (timer >= 0.3f)
            //    {
            //    yield break;

            //    }
            //}
            yield return null;
        }
        //while (timer <= 1f)
        //{
        //    timer += Time.fixedDeltaTime;
        //    transform.position = new Vector3(transform.position.x, transform.position.y - 55 * Time.deltaTime, transform.position.z);
        //    if (isGrounded)
        //        {
        //            yield break;
        //        }
        //    yield return null;
        //}

        //Debug.Log("pressed");
        //float timer = 0;

        //var startPos = Mathf.Abs(transform.position.y);
        //var startPosHor = Mathf.Sqrt(startPos);
        //while (timer <= 10)
        //{
        //    float jumpPosition = -Mathf.Pow((4 * timer /*- startPosHor*/), 2) + startPos;
        //    //float jumpPosition = -Mathf.Pow(timer,2) + 3;
        //    transform.position = new Vector3(transform.position.x, jumpPosition, transform.position.z);
        //    Debug.Log($"{jumpPosition}");
        //    timer += Time.fixedDeltaTime;
        //    if (isGrounded)
        //    {
        //        yield break;
        //    }
        //    yield return null;
        //}
    }
    private void Gravity()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, transform.lossyScale.y / 2, 1 << 6);
        if (hit)
        {
            isGrounded = true;
        }
        else
        {
            transform.position += Vector3.down * gravity;
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * transform.lossyScale.y / 2);
    }
}
