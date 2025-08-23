using System;
using System.Collections.Generic;
using RareChip.AITools;
using UnityEngine;

namespace RareChip.Goap
{
    public abstract class GoapConfig : ScriptableObject
    {
        public abstract void ConfigureConditions(GoapAgent agent, IContext context);

        public virtual List<GoapAction> ConfigureActions(GoapAgent agent, IContext context)
        {
            // noop
            return null;
        }

        public virtual List<IGoal> ConfigureGoals(GoapAgent agent, IContext context)
        {
            // noop
            return null;
        }
    }
}