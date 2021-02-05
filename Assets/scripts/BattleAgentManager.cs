using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerState
{
    public int playerIndex;
    [FormerlySerializedAs("agentRB")]
    public Rigidbody agentRb;
    public Vector3 startingPos;
    public BattleAgent agentScript;
}

public class BattleAgentManager : MonoBehaviour
{
    private int headCount;
    private int remainLives;
    public List<PlayerState> playerStates = new List<PlayerState>();
    public BattleAgentManager opponent;

    void Start() {
        // Debug.Log("playerStates " + playerStates);
    }

    public void CountAgent()
    {
        headCount += 1;
        remainLives += 1;
    }

    public void AgentDie(BattleAgent ba)
    {
        remainLives -= 1;
        // Debug.Log("m_BattleAgentManager.remainLives" + remainLives);
        // Destroy(ba.gameObject);
        Debug.Log("playerStates.Count" + playerStates.Count);
        if (remainLives == 0) {
            // Debug.Log("lost the game");
            // NoAgentInTeam()
            NoAgentInTeam();
        }
    }

    public void NoAgentInTeam() {
        foreach (var ps in playerStates)
        {
            Debug.Log("lost: ps.playerIndex" + ps.playerIndex);
            Debug.Log("ps.agentScript.team" + ps.agentScript.team);
            ps.agentScript.AddReward(-1.0f);
            ps.agentScript.EndEpisode();
        }
        foreach (var ps in opponent.playerStates)
        {
            Debug.Log("win: ps.playerIndex" + ps.playerIndex);
            Debug.Log("ps.agentScript.team" + ps.agentScript.team);
            ps.agentScript.AddReward(1.0f);
            ps.agentScript.EndEpisode();
        }
    }

}
