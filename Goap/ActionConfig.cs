using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace RareChip.Goap
{
    public abstract class ActionConfig : ScriptableObject
    {
        public string Name;
        public bool HasStaticCost = true;

        [SerializeField, ConditionalField("HasStaticCost")]
        public float Cost = 1f;

        public List<GoapCondition> requiredConditions;
        public List<GoapCondition> effects;

        public bool IsInterruptable = true;

        protected abstract IActionStrategy GetStrategy();

        public GoapAction GetAction(GoapAgent agent)
        {
            IActionStrategy strategy = GetStrategy();

            GoapAction action = new GoapAction.Builder(Name)
                .WithStrategy(strategy)
                .AddPreconditions(requiredConditions)
                .AddEffects(effects)
                .IsInterruptable(IsInterruptable)
                .WithStaticCost(HasStaticCost, Cost)
                .Build();

            return action;
        }
    }
}