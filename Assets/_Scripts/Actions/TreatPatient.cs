using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatPatient : GAction
{
    //go to cubicle
    public override bool PrePerform()
    {
        target = inventory.FindItemWithTag("Cubicle");
        if (target == null)
            return false;

        return true;
    }

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("TreatingPatient", 1); //Keep track how many patient treated
        GWorld.Instance.AddCubicle(target); //return the used cubicle back to cubicle list
        
        inventory.RemoveItem(target);
        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", 1); //Add free cubicle +1 in world state
        return true;
    }
}
