using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadEnvironment : MonoBehaviour {

    public GameObject loadingImage;

    public void LoadScene(string sceneName)
    {
        // PLAYER SETTINGS EXAMPLE
        LooterAgent.DEX = 3;
        LooterAgent.TIME = 60;
        LooterAgent.HP = 3;

        ArcherAgent.RANGE = 4f;
        ArcherAgent.FIRERATE = 0.1f;
        ArcherAgent.STR = 1;
        loadingImage.SetActive(true);
        SceneManager.LoadScene(sceneName);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
