using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// a node in the plan graph to be constructed
public class Node
{

    //the parent node this node is connected to
    public Node parent;
    //how much it cost to get to this node
    public float cost;
    //the state of the environment by the time the
    //action assigned to this node is achieved
    public Dictionary<string, int> state;
    //the action this node represents in the plan
    public GAction action;

    // Constructor
    public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
    {

        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }

    // Overloaded Constructor
    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, GAction action)
    {

        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);

        //as well as the world states add the agents beliefs as states that can be
        //used to match preconditions
        foreach (KeyValuePair<string, int> b in beliefStates)
        {

            if (!this.state.ContainsKey(b.Key))
            {

                this.state.Add(b.Key, b.Value);
            }
        }
        this.action = action;
    }
}

public class GPlanner
{

    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
    {

        List<GAction> usableActions = new List<GAction>();

        //of all the actions available find the ones that can be achieved.
        foreach (GAction a in actions)
        {

            if (a.IsAchievable())
            {

                usableActions.Add(a);
            }
        }

        //create the first node in the graph
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0.0f, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        //pass the first node through to start branching out the graph of plans from
        bool success = BuildGraph(start, leaves, usableActions, goal);

        //if a plan wasn't found
        if (!success)
        {

            Debug.Log("NO PLAN");
            return null;
        }

        //of all the plans found, find the one that's cheapest to execute
        //and use that
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {

            if (cheapest == null)
            {

                cheapest = leaf;
            }
            else if (leaf.cost < cheapest.cost)
            {

                cheapest = leaf;
            }
        }
        List<GAction> result = new List<GAction>();
        Node n = cheapest;

        while (n != null)
        {

            if (n.action != null)
            {

                result.Insert(0, n.action);
            }

            n = n.parent;
        }

        //make a queue out of the actions represented by the nodes in the plan
        //for the agent to work its way through
        Queue<GAction> queue = new Queue<GAction>();

        foreach (GAction a in result)
        {

            queue.Enqueue(a);
        }

        Debug.Log("The Plan is: ");
        foreach (GAction a in queue)
        {

            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {

        bool foundPath = false;

        //with all the useable actions
        foreach (GAction action in usableActions)
        {

            //check their preconditions
            if (action.IsAchievableGiven(parent.state))
            {

                //get the state of the world if the parent node were to be executed
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                //add the effects of this node to the nodes states to reflect what
                //the world would look like if this node's action were executed
                foreach (KeyValuePair<string, int> eff in action.effects)
                {

                    if (!currentState.ContainsKey(eff.Key))
                    {

                        currentState.Add(eff.Key, eff.Value);
                    }
                }

                //create the next node in the branch and set this current node as the parent
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                //if the current state of the world after doing this node's action is the goal
                //this plan will achieve that goal and will become the agent's plan
                if (GoalAchieved(goal, currentState))
                {

                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    //if no goal has been found branch out to add other actions to the plan
                    List<GAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);

                    if (found)
                    {

                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    //remove and action from a list of actions
    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)
    {

        List<GAction> subset = new List<GAction>();

        foreach (GAction a in actions)
        {

            if (!a.Equals(removeMe))
            {

                subset.Add(a);
            }
        }
        return subset;
    }

    //check goals against state of the world to determine if the goal has been achieved.
    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {

        foreach (KeyValuePair<string, int> g in goal)
        {

            if (!state.ContainsKey(g.Key))
            {

                return false;
            }
        }
        return true;
    }
}

/*
//ORIGINAL
public class Node //constructing a graph (set of nodes) for the planner
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action; //the action that a node is having

    public Node (Node parent, float cost, Dictionary<string, int> allstates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates); //make a copy of the allstates dict
        this.action = action; //Define the action!!!!
    }

    //overloaded Node that takes belief in
    public Node(Node parent, float cost, Dictionary<string, int> allstates, Dictionary<string, int> beliefstates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates); //make a copy of the allstates dict
        foreach (KeyValuePair<string, int> b in beliefstates)
            if (!this.state.ContainsKey(b.Key))
                this.state.Add(b.Key, b.Value);

        this.action = action; //Define the action!!!!
    }
}

public class GPlanner
{
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefstates) //method that returning queue of GActions, require the list of actions, dict of goal, and states of the worldstates
    {
        //find out actions that usable
        List<GAction> usableActions = new List<GAction>();
        foreach(GAction a in actions)  //looping through the actions to add them to the list of usable actions
        {
            if (a.IsAchievable())  //add only the actions that achieveable
                usableActions.Add(a);
        }

        //set up list of the graph
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0.0f, GWorld.Instance.GetWorld().GetStates(), beliefstates.GetStates(), null);  //a node that has no parent, with cost of 0, and copy the list from GWorld state, without action

        bool success = BuildGraph(start, leaves, usableActions, goal);  //WE HAVENT BUILT THIS

        if(!success)
        {
            //Debug.Log("NO PLAN");
            return null;  //tell the plan is empty
        }

        Node cheapest = null;
        foreach (Node leaf in leaves) //find the cheapest leaf on the leaves node
        {
            if (cheapest == null)
                cheapest = leaf; //use whatever if there is no cheapest leaf
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }

        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while (n != null)
        {
            if(n.action!= null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        //queue of GAction from the list of action that agent can perform
        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result)
        {
            queue.Enqueue(a); //put the item from list to the bottom of the queue

        }

        //Debug.Log("The Plan is: ");
        foreach (GAction a in queue) //list all the queue of GAction in console
        {
            //Debug.Log("Q: " + a.actionName);
        }

        return queue; //return back the queue to the agent so it has the plan
    }

    //recursion -> method call itself
    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal) //graph from a parent node, consisting the list of leaves, list of actions, and list of goals.
    {
        bool foundPath = false;  //no path at the beginning
        foreach (GAction action in usableActions) //loop the GAction in usable actions to check:
        {
            if(action.IsAchievableGiven(parent.state)) //if the actions is achievablegiven(see GAction), create a dictionary of currentstate
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);  //copied dict from parent state
                foreach(KeyValuePair<string, int> eff in action.effects)  //keep track of every achiveable states
                {
                    if (!currentState.ContainsKey(eff.Key))  //effect of current actions
                        currentState.Add(eff.Key, eff.Value);
                }

                Node node = new Node(parent, parent.cost + action.cost, currentState, action);  //create next node with accumulative cost from the previous node
                
                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else //if we can't find the path
                {
                    List<GAction> subset = ActionSubset(usableActions, action); //makes the usable action smaller along the way
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
        }
        return foundPath; //return foundPath value to the planner
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)  //Passing through, looking forward, and remove the action subset
    {
        List<GAction> subset = new List<GAction>();
        foreach (GAction a in actions)
        {
            if (!a.Equals(removeMe)) //check if the action that need to be remove isn't found
                subset.Add(a);
        }

        return subset;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach(KeyValuePair<string, int> g in goal) //loop the goal, make sure it's there
        {
            if (!state.ContainsKey(g.Key))
                return false;  //return false if no key found
        }
        return true;
    }
}
*/