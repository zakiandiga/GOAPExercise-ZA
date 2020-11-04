using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
}

public class WorldStates
{
    public Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    public bool hasState(string key)  //check if theres a key
    {
        return states.ContainsKey(key);
    }

    void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    public void ModifyState(string key, int value) //change the current state
    {
        if (states.ContainsKey(key))  //if currently there is a state available,
        {
            states[key] += value;  //increase the index
            if (states[key] <= 0)  // if its the 0 or lower index
                RemoveState(key);  //see below
        }
        else
            states.Add(key, value); //if there is no previous state, then add one

    }
    
    public void RemoveState(string key) //Remove the previous state if there's any
    {
        if (states.ContainsKey(key))
            states.Remove(key);
    }

    public void SetState(string key, int value) //Set the current state
    {
        if (states.ContainsKey(key))
            states[key] = value;
        else
            states.Add(key, value);
    }

    public Dictionary<string,int> GetStates() //Return the current state
    {
        return states;
    }
}
