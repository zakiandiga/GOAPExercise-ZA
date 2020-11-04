using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GAgent
{
    private int restCount = 0;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); //call the start on script this inherit from (GAgent script)
        SubGoal s1 = new SubGoal("treatPatient", 3, false); //set the iswaiting goal at priority of 1 and is removeable at start
        goals.Add(s1, 3); //add s1 to the goal list

        SubGoal s2 = new SubGoal("rested", 1, false); 
        goals.Add(s2, 1);

        //SubGoal s3 = new SubGoal("readyToTreat", 2, false);
        //goals.Add(s3, 2);

        Invoke("GetTired", Random.Range(20, 30));
        //beliefs.ModifyState("readyToTreat", 0);
    }
    

    void GetTired()
    {
        /*
        restCount += 1;
        if(restCount >= 3)
        {
            Invoke("GetBored", 0);
        }
        else
        */
        //beliefs.RemoveState("readyToTreat");
        beliefs.ModifyState("exhausted", 0);
        Invoke("GetTired", Random.Range(20,30));
        Debug.Log("GetTired Invoked");
                
    }

    /*
    void GetBored()
    {
        beliefs.ModifyState("bored", 0);
        Invoke("GetTired", Random.Range(5, 10));
    }

    */
}
