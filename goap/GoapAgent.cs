using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public class GoapAgent : MonoBehaviour
    {
        private GoapAgentConfiguration goapConfiguration;
        private HashSet<GoapAction> actions;
        private HashSet<GoapGoal> goals;

        private GoapPlanner planner;
        private GoapGoal currentGoal;
        private Queue<GoapAction> currentPlan;
        private void Awake()
        {
            actions = goapConfiguration.GetActions();
            goals = goapConfiguration.GetGoals();
            planner = new GoapPlanner();
            currentPlan = new Queue<GoapAction>();
        }

        private void Update()
        {
            
            planner.GeneratePlan(actions, currentGoal);
        }

    }
}