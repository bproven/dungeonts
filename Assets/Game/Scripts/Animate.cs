using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour {

    private Vector2 lastPos;
    private SpriteRenderer mySprite;
    Animator anim;

    // Use this for initialization
    void Start () {
        lastPos = gameObject.transform.position;

        mySprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update () {
        Vector2 curPos = gameObject.transform.position;
        if (curPos == lastPos) {
            anim.SetTrigger("idle");
        }
        else
        {
            anim.SetTrigger("run");
            Vector2 direction = (curPos - lastPos);
            if (direction.x < 0)
                mySprite.flipX = true;
            else
                mySprite.flipX = false;
        }
        lastPos = curPos;
    }
}
