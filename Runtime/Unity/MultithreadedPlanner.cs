using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.Unity
{
    public class MultithreadedPlanner
    {
        private readonly IActionPlanner actionPlanner;
        private readonly ISet<GoapAction> actions;
        private readonly ISet<GoapGoal> goals;
        private Task<IActionPlan> planTask;
        public MultithreadedPlanner(IActionPlanner planner, ISet<GoapAction> actions, ISet<GoapGoal> goals)
        {
            this.actionPlanner = planner;
            this.actions = actions;
            this.goals = goals;
        }
        public bool SchedulePlanRequest(GoapGoal goal, IWorldState worldState)
        {
            if (planTask != null)
                return false;
            
            planTask = GeneratePlanAsync(goal, worldState);
            return true;
        }

        public bool GetNewPlanIfReady(out IActionPlan plan)
        {
            if (planTask != null && (planTask.IsCanceled || planTask.IsFaulted))
            {
                throw planTask.Exception?.InnerException ?? new Exception("Multithreaded planning failed!");
            } 
            
            if (planTask == null || !planTask.IsCompleted)
            {
                plan = null;
                return false;
            }

            plan = planTask.Result;
            planTask.Dispose();
            planTask = null;
            return true;
        }

        private async Task<IActionPlan> GeneratePlanAsync(GoapGoal goal, IWorldState worldState)
        {
            IActionPlan plan = await Task.Run(() => actionPlanner.GeneratePlan(actions, goal, worldState));
            return plan;
        }
    }
    
}