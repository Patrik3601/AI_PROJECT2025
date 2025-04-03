using System;
using Assets._Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Sentis;
using UnityEngine;
using System.IO;

public class PlayerAgent : Agent
{
    public Motion motion;
    public ParentPlayerScript parentScript;

    public EventHandler OnNextGeneration;
    public GameObject targetPosition;

    public Vector3 startPos;
    private float lastJumpTime = 0f;
    private float jumpCooldown = 0.0f; // Cooldown for jump
    private bool isJumping = false;
    private float jumpStartX = 0f;
    private float jumpDistanceThreshold = 2.0f; // Minimum horizontal distance to reward a long jump

    private float epsilon = 0.00f;  // Exploration rate (10% chance to explore)
    private float epsilonDecay = 0.99999f;  // Decay factor for epsilon
    private float minEpsilon = 0.00f;  // Minimum epsilon value

    private void LogTrainingData(int moveX, int jump)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "training_data.csv");
        Debug.Log(filePath);
        string data = $"{transform.position.x},{transform.position.y},{motion.rb.linearVelocityY}," +
                      $"{motion.xSpeed},{(motion.canJump ? 1 : 0)},{(motion.isGrounded ? 1 : 0)}," +
                      $"{(motion.isStuck ? 1 : 0)},{(motion.isWallRight ? 1 : 0)},{(motion.isWallLeft ? 1 : 0)}," +
                      $"{(motion.isHoleRight ? 1 : 0)},{(motion.isHoleLeft ? 1 : 0)},{moveX},{jump}";

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "pos_x,pos_y,velocity_y,speed,can_jump,is_grounded,is_stuck,wall_right,wall_left,hole_right,hole_left,moveX,jump\n");
        }

        
        File.AppendAllText(filePath, data + "\n");
    }
    
    
    public override void Initialize()
    {
        //Time.timeScale = 50f; // Speed up the game for training
        
        // Subscribe to the player's death event
        parentScript.OnPlayerDead += HandleDeath;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0, -12, 0); // Reset position
        startPos = transform.position; // Set the start position

        OnNextGeneration?.Invoke(this, EventArgs.Empty); // Notify of new generation
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
        // Optional: Manual control logic for testing
    }

    public override void OnActionReceived(ActionBuffers actions)
{
    // Decide whether to explore (random action) or exploit (best-known action)
    if (UnityEngine.Random.value < epsilon)
    {
        actions.DiscreteActions.Array[0] = UnityEngine.Random.Range(0, 2);  // Random jump action (0 or 1)
        actions.DiscreteActions.Array[1] = UnityEngine.Random.Range(0, 3);  // Random move action (0, 1, or 2)
    }
    else
    {
        // Exploitation: Follow normal logic (best-known action)
        int jump = actions.DiscreteActions[0]; // Jump input (single tap)
        int moveX = actions.DiscreteActions[1]; // Move left or right (hold for long jump)

        LogTrainingData(moveX, jump);
        
        Debug.Log(moveX);

        // Handle horizontal movement
        if (moveX == 1)
        {
            motion.Move(-1); // Move left
        }
        if (moveX == 2)
        {
            motion.Move(1); // Move right
        }

        // Forward movement reward
        float forwardMovement = transform.position.x - startPos.x;
        if (forwardMovement > 0)
        {
            AddReward(forwardMovement * 0.1f); // Reward for moving right
        }
        else
        {
            AddReward(-0.05f); // Small penalty for moving backward
        }

        // Jump handling
        if (jump == 1 && motion.isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            lastJumpTime = Time.time;
            motion.Jump();
            Debug.Log("Jump triggered!");  // Debugging jump trigger

            // Reward for long jump (holding right in the air)
            if (moveX == 2 && !motion.isGrounded)  // Right movement in the air
            {
                AddReward(0.2f); // Reward for holding right for long jump
                Debug.Log("Long Jump Triggered!");  // Debugging long jump trigger
            }
        }

        // Penalize unnecessary jumps (jumping when not needed)
        if (jump == 1 && !motion.isGrounded)
        {
            AddReward(-0.1f); // Penalize for jumping when not grounded
        }

        // Reward for successfully landing after a long jump
        if (motion.isGrounded && forwardMovement > 1.5f)
        {
            AddReward(0.5f); // Reward for successful long jump landing
        }

        // Penalize being stuck
        if (motion.isStuck)
        {
            AddReward(-0.1f * motion.stuckTimer - motion.stuckThresholdTime);
        }

        // Penalize jumping when it's not needed
        if (jump == 1 && !motion.canJump)
        {
            AddReward(-0.1f);
        }
    }

    // Epsilon decay over time
    epsilon = Mathf.Max(minEpsilon, epsilon * epsilonDecay);  // Gradually reduce epsilon
    Debug.Log(epsilon);
}
    private void HandleDeath(object sender, PlayerDeadEventArgs e)
    {
        SetReward(-200f); // Negative reward for dying
        EndEpisode();   // Restart episode after death
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            SetReward(500f); // Reward for reaching the goal
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
