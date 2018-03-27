using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    // Use this for initialization
    //void Start () {
    //	
    //}
    //
    // Update is called once per frame
    //void Update () {
    //	
    //}

    int map = 0;
    public void PlayGame()
    {
        SceneManager.LoadScene("HybridLooterShooter");
    }

    public void PlayMap1()
    {
        SceneManager.LoadScene("HybridLooterShooter");
    }

    public void PlayMap2()
    {
        SceneManager.LoadScene("HybridLooterShooter 1");
    }

    public void PlayMap3()
    {
        SceneManager.LoadScene("HybridLooterShooter");
    }

    public void selectSword()
    {
        ArcherAgent.STR = 2;
    }

    public void selectArmor()
    {
        LooterAgent.HP = 1;
    }

    public void selectBoots()
    {
        LooterAgent.DEX = 3;
    }



}
