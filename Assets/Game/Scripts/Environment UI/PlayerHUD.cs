using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour {

    public static int time, score;  // Time remaining in seconds, score from resultswindow
    public static float health;     // Ratio of current health to overall HP

    public int deb_Time, deb_Score;
    public float deb_Health;

	// Use this for initialization
	void Start () {

	}

    // Update the time string
    private void updateTimer()
    {

    }

    // Update the length of the Health Bar
    private void updateHealthBar()
    {

    }

    // Update the score
    private void updateScore()
    {

    }
	
	// Update is called once per frame
	void Update () {
        // Run updates.
        updateTimer();
        updateScore();
        updateHealthBar();

        // Debug
        deb_Health = health;
        deb_Score = score;
        deb_Time = time;
	}
}
