using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateAcademy : Academy {
    
    public override void AcademyReset()
    {
    }

    public override void AcademyStep()
    {


    }

    public void Continue()
    {
        foreach (Agent a in GameObject.FindObjectsOfType<Agent>())
            a.Done();
    }

}
