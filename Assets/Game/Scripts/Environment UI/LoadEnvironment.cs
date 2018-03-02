using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadEnvironment : MonoBehaviour {

    public GameObject loadingImage;

    public void LoadScene(string sceneName)
    {
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
