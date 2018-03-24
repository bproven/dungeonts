using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    private List<Vector3> myState = new List<Vector3>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
            myState.Add(transform.GetChild(i).position);
	}
	
    public void resetLevel()
    {
        for (int i = 0; i < myState.Count; i++)
        {
            transform.GetChild(i).position = myState[i];
            transform.GetChild(i).gameObject.SetActive(true);
            if (transform.GetChild(i).GetComponent<AttackPlayer>())
                transform.GetChild(i).GetComponent<AttackPlayer>().reset();
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
