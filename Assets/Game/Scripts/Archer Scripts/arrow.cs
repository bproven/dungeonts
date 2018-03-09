using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour {

    public float spawnTime;

	// Use this for initialization
	void Awake () {
        spawnTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
		if (Time.time - spawnTime > 5)
        {
            Destroy(gameObject);
        }
        gameObject.transform.position += gameObject.transform.up * Time.deltaTime * 3;
	}
}
