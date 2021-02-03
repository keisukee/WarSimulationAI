using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class BattleAgent : Agent
{
    Rigidbody m_AgentRb;

    public GameObject area;
    public Bounds areaBounds;
    public float wallThickness = 0f;
    bool m_Shoot;
    bool m_Frozen;
    float m_FrozenTime;
    float m_EffectTime;
    float m_LaserLength = 1.0f;

    public Material normalMaterial;
    public Material frozenMaterial;
    public GameObject myLaser;

    // Speed of agent rotation.
    public float turnSpeed = 300;
    // Speed of agent movement.
    public float moveSpeed = 2;

    EnvironmentParameters m_ResetParams;

    void Start() {
        m_AgentRb = GetComponent<Rigidbody>();
        areaBounds = area.GetComponent<Collider>().bounds;
        Debug.Log("areaBounds " + areaBounds);
        Debug.Log("areaBounds.min " + areaBounds.min);
        Debug.Log("areaBounds.max " + areaBounds.max);
    }

    public override void OnEpisodeBegin() {
        Unfreeze();
        m_Shoot = false;
        m_AgentRb.velocity = Vector3.zero;
        myLaser.transform.localScale = new Vector3(0f, 0f, 0f);

        var randomPosX = Random.Range(-areaBounds.extents.x + wallThickness, areaBounds.extents.x - wallThickness);
        var randomPosZ = Random.Range(-areaBounds.extents.z + wallThickness, areaBounds.extents.z - wallThickness);
        var randomSpawnPos = Vector3.zero;
        var relativePosX = area.transform.position.x;
        var relativePosZ = area.transform.position.z;
        randomSpawnPos = new Vector3(randomPosX, 0.5f, randomPosZ);
        transform.position = new Vector3(relativePosX + randomPosX, 0.5f, relativePosZ + randomPosZ);
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);
        sensor.AddObservation(m_Frozen);
        sensor.AddObservation(m_Shoot);
    }

    public Color32 ToColor(int hexVal)
    {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers);

        // AddReward(-1f / MaxStep);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        m_Shoot = false;

        if (Time.time > m_FrozenTime + 4f && m_Frozen)
        {
            Unfreeze();
        }
        // if (Time.time > m_EffectTime + 0.5f)
        // {
        //     if (m_Poisoned)
        //     {
        //         Unpoison();
        //     }
        //     if (m_Satiated)
        //     {
        //         Unsatiate();
        //     }
        // }

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        if (!m_Frozen)
        {
            var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
            var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
            var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

            dirToGo = transform.forward * forward;
            dirToGo += transform.right * right;
            rotateDir = -transform.up * rotate;

            var shootCommand = (int)discreteActions[0] > 0;
            if (shootCommand)
            {
                Debug.Log("will shoot");
                m_Shoot = true;
                dirToGo *= 0.5f;
                m_AgentRb.velocity *= 0.75f;
            }
            m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        }

        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }

        if (m_Shoot)
        {
            var myTransform = transform;
            myLaser.transform.localScale = new Vector3(1f, 1f, m_LaserLength);
            var rayDir = 20.0f * myTransform.forward;
            Debug.DrawRay(myTransform.position, rayDir, Color.red, 0f, true);
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 2f, rayDir, out hit, 20.0f))
            {
                if (hit.collider.gameObject.CompareTag("agent"))
                {
                    AddReward(1f);
                    hit.collider.gameObject.GetComponent<BattleAgent>().Freeze();
                }
            }
        }
        else
        {
            myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    void Freeze()
    {
        gameObject.tag = "frozenAgent";
        AddReward(-1f);
        m_Frozen = true;
        m_FrozenTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = frozenMaterial;
    }

    void Unfreeze()
    {
        m_Frozen = false;
        gameObject.tag = "agent";
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = 0;
        continuousActionsOut[1] = 0;
        continuousActionsOut[2] = 0;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public void SetLaserLengths()
    {
        m_LaserLength = 2.0f;
    }

    public void SetAgentScale()
    {
        // float agentScale = m_ResetParams.GetWithDefault("agent_scale", 1.0f);
        // gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
    }

    public void SetResetParameters()
    {
        SetLaserLengths();
        SetAgentScale();
    }

}