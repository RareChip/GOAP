using System.Collections.Generic;
using GOAP.Core.Planning;
using UnityEngine;

namespace GOAP.Core.Agent
{
    public class GoapAgent : MonoBehaviour
    {
        private GoapAgentConfiguration goapConfiguration;
        private HashSet<GoapAction> actions;
        private HashSet<GoapGoal> goals;
        private WorldState worldState;
        
        private GoapPlanner planner;
        private GoapGoal currentGoal;
        private ActionPlan currentPlan;
        private void Awake()
        {
            worldState = goapConfiguration.CreateWorldState();
            actions = goapConfiguration.CreateActions(new GoapActionBuilder());
            goals = goapConfiguration.CreateGoals();
            planner = new GoapPlanner();
        }

        private void Update()
        {
            //currentPlan = planner.GeneratePlan(actions, currentGoal, worldState);
        }

    }
}