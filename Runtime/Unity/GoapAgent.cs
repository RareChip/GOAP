using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Unity
{
    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private GoapAgentConfiguration goapConfiguration;
        [SerializeField] private int maxPlanLength = 20;
        [SerializeField] private float planningInterval = 0.5f;

        private HashSet<GoapAction> actions;
        private HashSet<GoapGoal> goals;
        private WorldState worldState;
        private MultithreadedPlanner planner;
        private GoapAction currentAction;
        private IActionPlan currentPlan;
        private float currentTime;

        private void Awake()
        {
            this.worldState = this.goapConfiguration.CreateWorldState();
            this.actions = this.goapConfiguration.CreateActions(new GoapActionBuilder());
            this.goals = this.goapConfiguration.CreateGoals();
            this.planner = new MultithreadedPlanner(new GoapForwardPlanner(maxPlanLength), actions, goals);
            this.currentTime = 0;
        }

        private void Update()
        {
            if (currentTime <= 0)
            {
                RequestNewPlan();
                currentTime = planningInterval;
            }
            else
            {
                currentTime -= Time.deltaTime;
            }

            if (currentPlan != null)
            {
                if (currentAction == null || currentAction.IsComplete)
                {
                    currentAction?.StopAction();
                    currentAction = currentPlan.PopAction();
                    currentAction?.StartAction();
                }
                
                currentAction?.ExecuteAction();
            }

            if (!planner.GetNewPlanIfReady(out IActionPlan newPlan))
                return;

            bool shouldUseNewPlan =
                currentPlan == null || !currentPlan.Goal.Equals(newPlan.Goal) || currentPlan.IsComplete ||
                newPlan.TotalCost < currentPlan.TotalCost;

            if (!shouldUseNewPlan) 
                return;
            
            currentPlan = newPlan;
            PrintActionPlan();
        }

        private void RequestNewPlan()
        {
            IWorldState worldStateSnapshot = worldState.CreateSnapshot();
            GoapGoal bestGoal = planner.GetBestGoalMainThread(worldStateSnapshot);

            planner.SchedulePlanRequest(bestGoal, worldStateSnapshot);
        }

        public void ForceReplan()
        {
            currentPlan = null;
            currentTime = planningInterval;
            RequestNewPlan();
        }

        private void PrintActionPlan()
        {
            string printString = "";
            printString += $"Goal: [{currentPlan.Goal.GoalName}]\n";
            printString += "Cost: " + currentPlan.TotalCost + "\n";
            printString += "Actions: ";
            bool first = true;
            foreach (GoapAction action in currentPlan.Actions)
            {
                if (!first)
                    printString += " -> ";
                first = false;
                printString += $"[{action.ActionName}]";
            }

            print(printString);
        }
    }
}