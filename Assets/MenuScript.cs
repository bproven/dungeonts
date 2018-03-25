using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

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
    public void PlayGame ()
    {
        SceneManager.LoadScene("HybridLooterShooter");
    }

}
