using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    private List<Vector3> myState = new List<Vector3>();
    public GameObject TempSpawns;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
            myState.Add(transform.GetChild(i).position);
	}
	
    public void resetLevel()
    {
        for (int i = 2; i < myState.Count; i++)
        {
            // Reset all objects' positions, with a small randomness factor
            transform.GetChild(i).position = myState[i];
            transform.GetChild(i).gameObject.SetActive(true);
            if (transform.GetChild(i).GetComponent<AttackPlayer>())
                transform.GetChild(i).GetComponent<AttackPlayer>().reset();
        }
        /*for (int i = 0; i < TempSpawns.transform.childCount; i++)
        {
            Destroy(TempSpawns.transform.GetChild(i).gameObject);
        }*/
    }

	// Update is called once per frame
	void Update () {
		
	}
}
