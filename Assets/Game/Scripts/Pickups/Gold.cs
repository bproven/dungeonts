using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour {

    public int value;
    public float respawnRange;

    public bool respawn;

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
        // In the future, the loot drops might scale differently
        value = Random.Range(5, 5);

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void toggleGold()
    {
        GetComponent<Collider2D>().enabled = !GetComponent<Collider2D>().enabled;
        GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
    }

    private void pickupGold(GameObject looter)
    {
        looter.GetComponent<Gold>().value += value;
        for (int i = 0; i < looter.transform.childCount; i++)
            if (looter.transform.GetChild(i).GetComponent<LooterAgent>())
                looter.transform.GetChild(i).GetComponent<LooterAgent>().reward += value;
        if (respawn)
            randomizeGold();
        else
            toggleGold();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Player")
            pickupGold(coll.gameObject);
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.tag == "Player")
            pickupGold(coll.gameObject);
    }
}
