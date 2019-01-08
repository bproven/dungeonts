using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCam : MonoBehaviour {

    private GameObject Player;

	// Use this for initialization
	void Awake () {
        Player = GameObject.Find("Player");
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z), 0.1f);
	}
}
