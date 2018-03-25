using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterClicker : MonoBehaviour {

    public GameObject monster;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject newMonster = GameObject.Instantiate(monster);
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            newMonster.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            newMonster.GetComponent<AttackPlayer>().player = Camera.main.transform.parent.gameObject;
            newMonster.transform.SetParent(transform);
        }
    }
}
