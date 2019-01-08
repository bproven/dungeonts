using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour {

    private Vector2 lastPos;
    Animator anim;
	public Vector2 enemy;
	public bool hit;
	private AudioSource aud;

    // Use this for initialization
    void Start () {
        lastPos = gameObject.transform.position;

        anim = GetComponent<Animator>();
		aud = GetComponent<AudioSource>();
		hit = false;
    }

	// Update is called once per frame
	void Update () {
		if (tag == "Player") {
			Vector2 curPos = gameObject.transform.position;
			if (curPos == lastPos) {
				anim.SetTrigger("idle");
			}
			else {
				Vector2 direction = (curPos - lastPos);
				float plus = direction.x + direction.y; float minus = direction.x - direction.y;
				anim.SetFloat("plus", plus);
				anim.SetFloat("minus", minus);

			}
			lastPos = curPos;

			if (hit) {
				float enemyPlus = enemy.x + enemy.y; float enemyMinus = enemy.x - enemy.y;
				anim.SetFloat("enemyPlus", enemyPlus);
				anim.SetFloat("enemyMinus", enemyMinus);
				anim.SetTrigger("attack");
				aud.Play();
			}
			hit = false;
		}
    }
}
