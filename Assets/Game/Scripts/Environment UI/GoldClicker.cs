using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldClicker : MonoBehaviour {
    public GameObject gold;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            GameObject newGold = GameObject.Instantiate(gold);
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            newGold.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
	}
}
