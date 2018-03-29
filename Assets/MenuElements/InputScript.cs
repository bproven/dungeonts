using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputScript : MonoBehaviour
{

    public void GetInput(string time)
    {
        //Debug.Log(time);
        Debug.Log(Convert.ToInt32(time));
        LooterAgent.TIME = Convert.ToInt32(time);
    }
}
