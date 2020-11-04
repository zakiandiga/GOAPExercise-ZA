using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientCheckOut : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //talking occurs here
        return true;
    }
}
