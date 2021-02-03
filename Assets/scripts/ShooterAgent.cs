// using System.Collections.Generic;
// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;
// using Unity.MLAgents.Actuators;
// using Random = UnityEngine.Random;

// public class ShooterAgent : Agent
// {
//     // Note that that the detectable tags are different for the blue and purple teams. The order is
//     public enum Team
//     {
//         Green = 0,
//         Red = 1
//     }
//     public Team team;
//     int m_PlayerIndex;

//     Rigidbody m_AgentRb;

//     public GameObject area;
//     public Bounds areaBounds;
//     public float wallThickness = 0f;

//     float m_PlayTime = 0;
//     // Speed of agent rotation.
//     public float turnSpeed = 300;
//     // Speed of agent movement.
//     public float moveSpeed = 2;

//     BehaviorParameters m_BehaviorParameters;
//     ShootingSettings m_ShootingSettings;
//     Vector3 m_Transform;

//     void Start() {
//         m_AgentRb = GetComponent<Rigidbody>();
//         areaBounds = area.GetComponent<Collider>().bounds;
//         Debug.Log("areaBounds " + areaBounds);
//         Debug.Log("areaBounds.min " + areaBounds.min);
//         Debug.Log("areaBounds.max " + areaBounds.max);

//         if (m_BehaviorParameters.TeamId == (int)Team.Green)
//         {
//             team = Team.Green;
//             m_Transform = new Vector3(transform.position.x - 4f, .5f, transform.position.z);
//         }
//         else
//         {
//             team = Team.Red;
//             m_Transform = new Vector3(transform.position.x + 4f, .5f, transform.position.z);
//         }

//     }

//     public Transform Target;

//     public override void OnEpisodeBegin() {
//         m_PlayTime = 0f;
//         if (this.transform.localPosition.y < 0) {
//             this.m_AgentRb.angularVelocity = Vector3.zero;
//             this.m_AgentRb.velocity = Vector3.zero;
//             this.transform.localPosition = new Vector3(0, 0.5f, 0);
//         }

//         var randomPosX = Random.Range(-areaBounds.extents.x + wallThickness, areaBounds.extents.x - wallThickness);
//         var randomPosZ = Random.Range(-areaBounds.extents.z + wallThickness, areaBounds.extents.z - wallThickness);
//         var randomSpawnPos = Vector3.zero;

//         randomSpawnPos = new Vector3(randomPosX, 0.5f, randomPosZ);

//         GameObject targetObject = Target.gameObject;
//         Rigidbody targetObjectRb = targetObject.GetComponent<Rigidbody>();
//         targetObjectRb.velocity = Vector3.zero;

//         Target.localPosition = randomSpawnPos;
//     }

//     public override void CollectObservations(VectorSensor sensor)
//     {
//         var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
//         // sensor.AddObservation(localVelocity.x);
//         // sensor.AddObservation(localVelocity.z);
//     }

//     public override void OnActionReceived(ActionBuffers actionBuffers)
//     {
//         MoveAgent(actionBuffers);
//         // // Rewards
//         // float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
//         // Debug.Log("distanceToTarget" + distanceToTarget);

//         // // Fell off platform
//         if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 100f)
//         {
//             EndEpisode();
//         }
//         AddReward(-1f / MaxStep);
//     }

//     public void FixedUpdate() {
//         m_PlayTime += Time.fixedDeltaTime;
//     }

//     public void MoveAgent(ActionBuffers actionBuffers)
//     {
//         var dirToGo = Vector3.zero;
//         var rotateDir = Vector3.zero;

//         var continuousActions = actionBuffers.ContinuousActions;
//         var discreteActions = actionBuffers.DiscreteActions;

//         var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
//         var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
//         var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

//         dirToGo = transform.forward * forward;
//         dirToGo += transform.right * right;
//         rotateDir = -transform.up * rotate;

//         m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
//         transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

//         if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
//         {
//             m_AgentRb.velocity *= 0.95f;
//         }

//     }

//     void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("target"))
//         {
//             AddReward(2f);
//             EndEpisode();
//         }
//     }

//     public override void Heuristic(in ActionBuffers actionsOut)
//     {
//         var continuousActionsOut = actionsOut.ContinuousActions;
//         continuousActionsOut[0] = 0;
//         continuousActionsOut[1] = 0;
//         continuousActionsOut[2] = 0;
//         if (Input.GetKey(KeyCode.D))
//         {
//             continuousActionsOut[2] = 1;
//         }
//         if (Input.GetKey(KeyCode.W))
//         {
//             continuousActionsOut[0] = 1;
//         }
//         if (Input.GetKey(KeyCode.A))
//         {
//             continuousActionsOut[2] = -1;
//         }
//         if (Input.GetKey(KeyCode.S))
//         {
//             continuousActionsOut[0] = -1;
//         }
//         var discreteActionsOut = actionsOut.DiscreteActions;
//         discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
//     }
// }
