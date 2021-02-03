using UnityEngine;
// using Unity.MLAgentsExamples;

public class BattleArea : MonoBehaviour
{
    public float range;

    public void ResetBattleArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

    }

    public void ResetArea()
    {
    }
}
