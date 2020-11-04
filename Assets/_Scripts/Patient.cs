using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient : GAgent
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); //call the start on script this inherit from (GAgent script)
        SubGoal s1 = new SubGoal("isWaiting", 3, true); //set the iswaiting goal at priority of 1 and is removeable at start
        goals.Add(s1, 3); //add s1 to the goal list

        SubGoal s2 = new SubGoal("isTreated", 5, true);
        goals.Add(s2, 5);

        SubGoal s3 = new SubGoal("CheckedOut", 4, true);
        goals.Add(s3, 4);

        SubGoal s4 = new SubGoal("Left", 1, true);
        goals.Add(s4, 1);
    }


}
