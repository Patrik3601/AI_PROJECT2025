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
    public override void Initialize()
    {
        // Subscribe to death event
        parentScript.OnPlayerDead += HandleDeath;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0, 0, 0);

       // motion.rb.linearVelocity = Vector2.zero;
        //motion.isGrounded = false;
    
        OnNextGeneration?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        motion.CheckGround();

        sensor.AddObservation(transform.position);
        sensor.AddObservation(motion.rb.linearVelocity);
        sensor.AddObservation(motion.isGrounded ? 1 : 0);
        sensor.AddObservation(motion.IsWallInFront(-1) ? 1 : 0);
        sensor.AddObservation(motion.IsWallInFront(1) ? 1 : 0);
    }

   private float epsilon = 1.0f;  // Starting with full randomness (100%)
private float epsilonDecay = 0.99f;  // Decay rate for epsilon (randomness)

public override void OnActionReceived(ActionBuffers actions)
{
    Debug.Log("adasd");
    
    // Gradually reduce randomness (epsilon decay)
    if (StepCount < 100) // Initial phase with random moves
    {
        float randomMove = UnityEngine.Random.Range(-1f, 1f);
        bool randomJump = UnityEngine.Random.value > 0.5f;

        if (randomMove < 0 && !motion.IsWallInFront(-1))
        {
            motion.Move(-1);
        }
        else if (randomMove > 0 && !motion.IsWallInFront(1))
        {
            motion.Move(1);
        }

        if (randomJump && motion.isGrounded)
        {
            motion.Jump();
        }
    }
    else
    {
        // Use epsilon-greedy approach: With decreasing epsilon, rely more on the policy
        if (UnityEngine.Random.value < epsilon) // If random value is less than epsilon, take random actions
        {
            float randomMove = UnityEngine.Random.Range(-1f, 1f);
            bool randomJump = UnityEngine.Random.value > 0.5f;

            if (randomMove < 0 && !motion.IsWallInFront(-1))
            {
                motion.Move(-1);
            }
            else if (randomMove > 0 && !motion.IsWallInFront(1))
            {
                motion.Move(1);
            }

            if (randomJump && motion.isGrounded)
            {
                motion.Jump();
            }
        }
        else
        {
            // Normal behavior using the model
            float move = actions.ContinuousActions[0];
            bool jump = actions.DiscreteActions[0] > 0;

            if (move < 0 && !motion.IsWallInFront(-1))
            {
                motion.Move(-1);
            }
            else if (move > 0 && !motion.IsWallInFront(1))
            {
                motion.Move(1);
            }

            if (jump && motion.isGrounded)
            {
                motion.Jump();
            }
        }
    }

    // Decay epsilon for less randomness over time
    epsilon *= epsilonDecay;
}

    private void HandleDeath(object sender, PlayerDeadEventArgs e)
    {
        SetReward(-1f);  // Give a negative reward for dying
        EndEpisode();     // Restart the training episode
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            SetReward(1f);
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
