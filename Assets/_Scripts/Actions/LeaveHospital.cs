using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveHospital : GAction
{
    // Start is called before the first frame update
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Destroy(this.gameObject, 3f);
        return true;
    }
}
