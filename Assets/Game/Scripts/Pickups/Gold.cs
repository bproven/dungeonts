using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour {

    public int value;
    public float respawnRange;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void randomizeGold()
    {
        gameObject.transform.position = new Vector2(transform.parent.position.x + Random.Range(-respawnRange, respawnRange),
                                                    transform.parent.position.y + Random.Range(-respawnRange, respawnRange));
        // I know this is deterministic, but I'm dialing in the reward values.
        value = Random.Range(5,5);

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void toggleGold()
    {
        GetComponent<Collider2D>().enabled = !GetComponent<Collider2D>().enabled;
        GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            coll.GetComponent<Gold>().value += value;
            coll.transform.GetComponent<LooterAgent>().reward += value;
            toggleGold();
            //coll.GetComponent<LootAndShootAgent>().roundStart += 2;
            //randomizeGold();
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            coll.GetComponent<Gold>().value += value;
            coll.transform.GetComponent<LooterAgent>().reward += value;
            toggleGold();
            //coll.GetComponent<LootAndShootAgent>().roundStart += 2;
            //randomizeGold();
        }
    }
}
