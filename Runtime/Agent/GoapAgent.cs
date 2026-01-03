using System.Collections.Generic;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Agent
{
    public class GoapAgent : MonoBehaviour
    {
        private GoapAgentConfiguration goapConfiguration;
        private HashSet<GoapAction> actions;
        private HashSet<GoapGoal> goals;
        private WorldState worldState;
        
        private IGoapPlanner planner;
        private GoapGoal currentGoal;
        private ActionPlan currentPlan;
        private void Awake()
        {
            this.worldState = this.goapConfiguration.CreateWorldState();
            this.actions = this.goapConfiguration.CreateActions(new GoapActionBuilder());
            this.goals = this.goapConfiguration.CreateGoals();
            this.planner = new GoapRegressivePlanner();
        }

        private void Update()
        {
            //currentPlan = planner.GeneratePlan(actions, currentGoal, worldState);
        }

    }
}