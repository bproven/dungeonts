using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour {

    private Vector2 lastPos;
    private SpriteRenderer mySprite;
    Animator anim;
	public bool plus;
	public bool minus;


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
		else {
			Vector2 direction = (curPos - lastPos);
			float plus = direction.x + direction.y; float minus = direction.x - direction.y;
			anim.SetFloat("plus", plus);
			anim.SetFloat("minus", minus);

			/*
			if (plus > 0) anim.SetBool("plus", true);
			else anim.SetBool("plus", false);
			if (minus > 0) anim.SetBool("minus", true);
			else anim.SetBool("minus", false);
			anim.GetBool("plus");
			anim.GetBool("minus");
			*/
		}
        lastPos = curPos;
    }
}
