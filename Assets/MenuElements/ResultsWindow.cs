using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsWindow : MonoBehaviour
{
    public GameObject window;
    //public TextMeshPro tmpScoreText;
    public static int score;
    public static int monstersKilled;
    public static int treasuresFound;
    public static int itemsFound;
    public static float damageTaken;
    public Text scoreText;
    public Text monstersKilledText;
    public Text treasuresFoundText;
    public Text itemsFoundText;
    public Text damageTakenText;

    void Start()
    {
        window.SetActive(false);
        ResetScore();
        CalculateScore();
        UpdateText();
        //tmpScoreText = gameObject.AddComponent<TextMeshPro>();
        //tmpScoreText.text = "TMP Score: " + score.ToString();
    }

    public static void ResetScore()
    {
        score = 0;
        monstersKilled = 0;
        treasuresFound = 0;
        itemsFound = 0;
        damageTaken = 0;
    }

    private void UpdateText()
    {
        scoreText.text = "Score: " + score.ToString();
        monstersKilledText.text = "Monsters Killed: " + monstersKilled.ToString();
        treasuresFoundText.text = "Treasures Found: " + treasuresFound.ToString();
        itemsFoundText.text = "Items Found: " + itemsFound.ToString();
        damageTakenText.text = "Damage Taken: " + damageTaken.ToString();
        Debug.Log("Text Updated!");
    }

    private static void CalculateScore()
    {
        score = monstersKilled * 10 +
            treasuresFound * 5 +
            itemsFound * 25
            - (int)(damageTaken * 5f);
        PlayerHUD.score = score;
    }

    public static void Score_EnemyKilled()
    {
        monstersKilled++;
        CalculateScore();
    }

    public static void Score_DamageTaken(float damage)
    {
        damageTaken += damage;
        CalculateScore();
    }

    public static void Score_GatheredLoot(int value)
    {
        treasuresFound += value;
        CalculateScore();
    }

    public static void Score_ObtainedItem()
    {
        itemsFound++;
        CalculateScore();
    }

    public void Show()
    {
        if (window.activeInHierarchy)
            return;
        window.SetActive(true);
        UpdateText();
    }

    public void Hide()
    {
        window.SetActive(false);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu2");
    }

}
