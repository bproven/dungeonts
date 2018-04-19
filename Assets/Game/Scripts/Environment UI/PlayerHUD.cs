using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    public static int score;  // Time remaining in seconds, score from resultswindow
    public static float health;     // Ratio of current health to overall HP
    public static float time;

    public Text timeText;
    public Text scoreText;

    //public float fillAmount;
    public Image HPBarFill;

    public int deb_Score;
    public float deb_Time;
    public float deb_Health;

	// Use this for initialization
	void Start () {
        updateTimer();
        updateHealthBar();
        updateScore();
	}

    // Update the time string
    private void updateTimer()
    {
        timeText.text = time.ToString("0.00");
    }

    // Update the length of the Health Bar
    private void updateHealthBar()
    {
        HPBarFill.fillAmount = health;
        //Debug.Log(health.ToString());

        if(health < 0.7 && health > 0.4 && health != 0)
        {
            HPBarFill.color = Color.yellow;
        }
        else if (health < 0.4 && health != 0)
        {
            HPBarFill.color = Color.red;
        }
    }

    // Update the score
    private void updateScore()
    {
        scoreText.text = score.ToString();
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
