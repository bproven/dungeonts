using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSplitter : MonoBehaviour {

    public GameObject screenSplitter;
    public int count;

    void splitLevel(int count)
    {
        GameObject level = transform.GetChild(0).gameObject;
        for (int i = 1; i < count; i++)
        {
            GameObject newLevel = GameObject.Instantiate(level);
            screenSplitter.GetComponent<SplitScreenGenerator>().cams[i] = newLevel.transform.GetChild(0).GetChild(2).GetComponent<Camera>();
            newLevel.transform.GetChild(0).GetChild(2).tag = "Untagged";
            newLevel.transform.GetChild(0).GetChild(2).GetComponent<AudioListener>().enabled = false;
            newLevel.transform.position = level.transform.position + new Vector3(50 * i, 0);
            newLevel.transform.SetParent(transform);
        }
    }

	// Use this for initialization
	void Start () {
        splitLevel(screenSplitter.GetComponent<SplitScreenGenerator>().cams.Length);
        screenSplitter.GetComponent<SplitScreenGenerator>().splitScreens();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
