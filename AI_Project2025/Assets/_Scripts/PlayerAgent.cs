using System;
using Assets._Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    public Motion motion;
    public ParentPlayerScript parentScript;

    public EventHandler OnNextGeneration;
    public GameObject targetPosition;

    public Vector3 startPos;

    public override void Initialize()
    {
        // Subscribe to death event
        parentScript.OnPlayerDead += HandleDeath;
    }
    private void Update()
    {
        Debug.Log(GetCumulativeReward());
    }
    public override void OnEpisodeBegin()
    {
        float dist = Vector3.Distance(startPos, transform.position);
        AddReward(dist);

        //Debug.Log(GetCumulativeReward());
        transform.position = new Vector3(0, -12, 0);
        startPos = transform.position;
        // motion.rb.linearVelocity = Vector2.zero;
        //motion.isGrounded = false;

        OnNextGeneration?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        motion.IsGrounded();

        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetPosition.transform.position);
        sensor.AddObservation(motion.rb.linearVelocity);
        sensor.AddObservation(motion.IsGrounded() ? 1 : 0);
        sensor.AddObservation(motion.IsWallInFront(-1) ? 1 : 0);
        sensor.AddObservation(motion.IsWallInFront(1) ? 1 : 0);
        sensor.AddObservation(motion.IsThereHoleInFront() ? 1 : 0);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int jump = actions.DiscreteActions[0];
        int moveX = actions.DiscreteActions[1];
        //Debug.Log(moveX);

        //if (!motion.IsWallInFront(-1) && !motion.IsWallInFront(1))
        //{
        //    motion.Move(moveX);
        //}
        //if (motion.IsGrounded())
        //{
        //    motion.Jump();
        //}

        Debug.Log(moveX);
        if (moveX == 0)
        {
            //nothing
        }
        if (moveX == 1)
        {
            if (!motion.IsWallInFront(-1))
            {
                motion.Move(-1);

            }
        }
        if (moveX == 2)
        {
            if (!motion.IsWallInFront(1))
            {
                motion.Move(1);
            }
        }
        //Debug.Log("jump" + jump);

        //AddReward(0.01f);


        if (transform.position.x < startPos.x)
        {
            AddReward(-1f);
        }
        if (jump == 1 && motion.IsGrounded() && Time.time > lastJumpTime + jumpCooldown)
        {
            AddReward(0.5f);
            lastJumpTime = Time.time;
            motion.Jump();
        }
        else if (jump == 1 && motion.IsGrounded() && Time.time < lastJumpTime + jumpCooldown)
        {
            AddReward(-0.1f);

        }

        if (jump == 1 && !motion.IsGrounded())
        {
            AddReward(-0.1f);
        }
    }
    private float lastJumpTime = 0f;
    private float jumpCooldown = 0.25f; // 1-second cooldown


    private void HandleDeath(object sender, PlayerDeadEventArgs e)
    {
        AddReward(-10f);  // Give a negative reward for dying
        EndEpisode();     // Restart the training episode
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            //Debug.Log("entered");
            AddReward(20f);
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
