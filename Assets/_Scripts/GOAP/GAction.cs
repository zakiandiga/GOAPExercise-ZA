using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target;  //the action location
    public string targetTag; //to pick the target with tag
    public float duration = 0;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    public Dictionary<string, int> preconditions; //populated from value in WorldState[]
    public Dictionary<string, int> effects;

    public WorldStates agentBeliefs; //agent's internal set of state

    public GInventory inventory;
    public WorldStates beliefs;

    public bool running = false; //to make sure we only run 1 action at a time

    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        if (preConditions != null)  //add precondition list to the precon and aftereffect dictionary based on available worldstate
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }
        }

        inventory = this.GetComponent<GAgent>().inventory;
        beliefs = this.GetComponent<GAgent>().beliefs;
    }

    public bool IsAchievable() //make sure the goal can be achieved by the agent
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)  //looping through preconditions for particular actions
        {
            if (!conditions.ContainsKey(p.Key))  //make sure it match, return false if the precon doesn't met
                return false;
        }
        return true;

    }

    public abstract bool PrePerform(); //inherit from here, to customize actions (check and follow up)
    public abstract bool PostPerform();

}
