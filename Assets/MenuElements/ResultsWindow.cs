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
    public int score;
    public int monstersKilled;
    public int treasuresFound;
    public int itemsFound;
    public int damageTaken;
    public Text scoreText;
    public Text monstersKilledText;
    public Text treasuresFoundText;
    public Text itemsFoundText;
    public Text damageTakenText;

    void Start()
    {
        window.SetActive(false);
        score = 0;
        monstersKilled = 0;
        treasuresFound = 0;
        itemsFound = 0;
        damageTaken = 0;

        window.SetActive(false);
        scoreText.text = "Score: " + score.ToString();
        monstersKilledText.text = "Monsters Killed: " + monstersKilled.ToString();
        treasuresFoundText.text = "Treasures Found: " + treasuresFound.ToString();
        itemsFoundText.text = "Items Found: " + itemsFound.ToString();
        damageTakenText.text = "Damage Taken: " + damageTaken.ToString();

        //tmpScoreText = gameObject.AddComponent<TextMeshPro>();
        //tmpScoreText.text = "TMP Score: " + score.ToString();
    }

    public void Show()
    {
        window.SetActive(true);
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
