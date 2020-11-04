using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTreated : GAction
{
    public override bool PrePerform()
    {
        target = inventory.FindItemWithTag("Cubicle");
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("Treated", 1); //Keep track how many patient treated
        beliefs.ModifyState("recovered", 1);
        inventory.RemoveItem(target);
        return true;
    }

}
