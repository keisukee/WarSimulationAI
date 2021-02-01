using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public int number = 10;
    public float space = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < number; i++) {
            for (int j = 0; j < number; j++) {
                Vector3 pos = transform.position + new Vector3(i * space, 0, j * space);
                GameObject unit = (GameObject)Instantiate(unitPrefab, pos, transform.rotation);
            }
        }
    }

}
