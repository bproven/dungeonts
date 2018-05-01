using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterClicker : MonoBehaviour {

    public GameObject monster;
    private GameObject player;

	// Use this for initialization
	void Awake () {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject newMonster = GameObject.Instantiate(monster);
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            newMonster.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            newMonster.GetComponent<AttackPlayer>().player = player;
            newMonster.transform.SetParent(transform);
        }
    }
}
