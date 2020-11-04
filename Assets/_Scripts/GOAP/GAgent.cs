using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//class that defines goals
//Drop this to the agents game objects
public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove; //Some agent need to remove the goal(s) that the agents need to achieved (go to waiting room), we dont remove the nurses rest's btw

    public SubGoal(string s, int i, bool r)  //define the goal name, importance(int), and if its removable or continuous
    {
        sgoals = new Dictionary<string, int>();  //create the subgoal dict, define the name, the goal value, if the goal is removable or continuos
        sgoals.Add(s, i); 
        remove = r;
    }
}

public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>(); //list of actions that agents can do
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();  //dictionary of subgoals that agents need to achieve
    public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();

    GPlanner planner;  //get goals, list of actions, worldstates

    Queue<GAction> actionQueue;  //queueing the action 
    public GAction currentAction; //track what the agent currently doing
    SubGoal currentGoal;  //track the current goal of the agent
    
    public void Start()  //we'll call this from patient code
    {        
        GAction[] acts = this.GetComponents<GAction>();  //an array of actions
        foreach (GAction a in acts)  //get all the GActions to the agents, put it to this array
            actions.Add(a);

    }

    bool invoked = false; //by default false

    void CompleteAction()  //finishing current action
    {
        currentAction.running = false; //set bool running in GAction script to false, stop the invocation of CompleteAction (this function)
        currentAction.PostPerform();  //execute the PostPerform()
        invoked = false;
    }

    void LateUpdate()
    {

        if (currentAction != null && currentAction.running) //in the middle of action
        {
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position); //the behavior bugged with this
            if (currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f) //tell navmesh to end the action based on remaining distance //// distanceToTarget < 1f); //
            {
                if(!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration); //invoking 10sec later (lateupdate)
                    invoked = true;
                }
            }
            return;
        }
        
        //add the planner
        if (planner == null || actionQueue == null) //if agent doesn't have plan to work on
        {
            planner = new GPlanner();
            
            //Sort through the goal from the most to the least important
            var sortedGoals = from entry in goals orderby entry.Value descending select entry; //sort goal array, start from entry order by it's value in descending order

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) //loop each sortedgoals to call the plan the most important goal
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, beliefs); //here pass the state(agentbelief) but it's null at the beginning, change it to beliefs
                if (actionQueue != null) //If we have the plan (not null)
                {
                    currentGoal = sg.Key;
                    break;  //stop looking (the foreach loop) goal if we have the current goal
                    
                }
            }
        }

        //if we have nothing to do left in queue
        if (actionQueue != null && actionQueue.Count == 0)
        {
            if(currentGoal.remove) //if the goal is removable
            {
                goals.Remove(currentGoal);
            }
            planner = null;  //trigger getting new plan
        }

        //if we still have something todo in queue
        
        if (actionQueue != null && actionQueue.Count > 0)  //conditions never met???? because the action wasnt define at GPlanner
        {
            currentAction = actionQueue.Dequeue(); //remove the action at the top of the queue, put it to the currentAction
            if(currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "") //if there's no target and the tag is not empty string
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag); //set the target of the agent to move to

                if (currentAction.target != null) //if we have the target destination (e.g. cubicle available)
                {
                    currentAction.running = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position); //move agent to the position of the target
                }
                                    
            }
            else
            {
                actionQueue = null; //force agent to get a new plan
            }    
        }
    }
}
