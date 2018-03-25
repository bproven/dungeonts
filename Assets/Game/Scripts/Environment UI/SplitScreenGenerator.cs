using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenGenerator : MonoBehaviour {

    public Camera[] cams;

    public float w = 0, h = 0;
    
	// Use this for initialization
	void Start () {
        if (cams.Length == 0)
            return;
        int numRows = 0, numCols = 0;
        int numScreens = cams.Length;
        for (int i = 4; i >= 0; i--)
        {
            if (Mathf.Pow(i, 2) <= numScreens)
            {
                numRows = i;    numCols = i;
                break;
            }
        }
        w = 1f / numRows; h = 1f / numCols;
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
            {
                if (i * numCols + j >= numScreens)
                    continue;
                cams[i * numCols + j].rect = new Rect(j * w, i * h, w, h);
            }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
