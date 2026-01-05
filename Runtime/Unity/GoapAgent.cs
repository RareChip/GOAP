using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Unity
{
    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private int maxPlanLength = 20;
        
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
            this.planner = new GoapForwardPlanner(20);
        }

        private void Update()
        {
            //currentPlan = planner.GeneratePlan(actions, currentGoal, worldState);
        }

    }
}