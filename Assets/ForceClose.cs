using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceClose : MonoBehaviour {

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        // Temporary solution to stop crashes for the demo build.
        if (!Application.isEditor)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
