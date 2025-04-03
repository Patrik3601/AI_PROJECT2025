using System;
using Assets._Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Sentis;
using UnityEngine;

public class PlayerAgent : Agent
{
    public Motion motion;
    public ParentPlayerScript parentScript;

    public EventHandler OnNextGeneration;
    public GameObject targetPosition;

    public Vector3 startPos;
    public float lastRecord;
    public override void Initialize()
    {
        // Subscribe to death event
        parentScript.OnPlayerDead += HandleDeath;
    }
    //private void Update()
    //{
    //    //Debug.Log(GetCumulativeReward());
    //}
    public override void OnEpisodeBegin()
    {
        //float dist = Vector3.Distance(startPos, transform.position);
        //AddReward(dist);

        //Debug.Log(GetCumulativeReward());
        transform.position = new Vector3(0, -12, 0);
        startPos = transform.position;
        // motion.rb.linearVelocity = Vector2.zero;
        //motion.isGrounded = false;

        OnNextGeneration?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetPosition.transform.position);
        sensor.AddObservation(motion.rb.linearVelocityY);
        sensor.AddObservation(motion.xSpeed);
        sensor.AddObservation(motion.canJump ? 1 : 0);
        sensor.AddObservation(motion.isGrounded ? 1 : 0);
        sensor.AddObservation(motion.isStuck ? 1 : 0);
        sensor.AddObservation(motion.isWallRight ? 1 : 0);
        sensor.AddObservation(motion.isWallLeft ? 1 : 0);
        sensor.AddObservation(motion.isHoleRight ? 1 : 0);
        sensor.AddObservation(motion.isHoleLeft ? 1 : 0);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        //int xMove = 0;
        //if (Input.GetKey(KeyCode.A))
        //{
        //    xMove = 1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    xMove = 2;

        //}
        //int jump = 0;
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    jump = 1;
        //}
        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = jump;
        //discreteActions[1] = xMove;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int jump = actions.DiscreteActions[0];
        int moveX = actions.DiscreteActions[1];

        Debug.Log(moveX);
        if (moveX == 0)
        {

        }
        if (moveX == 1)
        {
            motion.Move(-1);
        }
        if (moveX == 2)
        {
            motion.Move(1);
        }

        if (transform.position.x < startPos.x)
        {
            AddReward(-1f);
        }
        if (jump == 1 && motion.isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            //AddReward(0.5f);
            lastJumpTime = Time.time;
            motion.Jump();
        }
        if (motion.isStuck)
        {
            AddReward(-0.1f * motion.stuckTimer - motion.stuckThresholdTime);
        }
        AddReward(Vector3.Distance(startPos, transform.position) * 0.003f);
        if (jump == 1 && !motion.canJump)
        {
            AddReward(-0.1f);
        }
    }
    private float lastJumpTime = 0f;
    private float jumpCooldown = 0.25f; // 1-second cooldown


    private void HandleDeath(object sender, PlayerDeadEventArgs e)
    {
        SetReward(-500f);  // Give a negative reward for dying
        EndEpisode();     // Restart the training episode
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            //Debug.Log("entered");
            SetReward(1000f);
            EndEpisode();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (parentScript != null)
        {
            parentScript.OnPlayerDead -= HandleDeath;
        }
    }
}
