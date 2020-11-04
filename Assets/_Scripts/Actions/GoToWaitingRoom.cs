using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToWaitingRoom : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("Waiting", 1);
        GWorld.Instance.AddPatient(this.gameObject); //assign this patient to the waiting state (so that the nurse know which to pick)
        beliefs.ModifyState("atHospital", 1); //inject belief to the patient
        return true;
    }

}