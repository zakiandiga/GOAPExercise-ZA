using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPatient : GAction
{
    GameObject resource;


    public override bool PrePerform()
    {
        target = GWorld.Instance.RemovePatient(); //mark patient on the queue
        if (target == null)
            return false; //make sure the nurse trigger get new action if theres no patients
        

        resource = GWorld.Instance.RemoveCubicle(); //Remove cube from the queue, reserve for this nurse
        if (resource != null)
            inventory.AddItem(resource); //grab the cube
        else
        {
            GWorld.Instance.AddPatient(target);
            target = null;
            return false;
        }

        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", - 1); //reduce the count of cubicle on queue by 1
        return true;
    }

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("Waiting", -1); //reduce the count of patient in the waiting room by 1
        if (target)
            target.GetComponent<GAgent>().inventory.AddItem(resource); //adding cubicle to the patient inventory
        return true;
    }

}