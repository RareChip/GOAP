using System.Collections.Generic;
using RareChip.AITools;

namespace RareChip.Goap
{
    public interface IActionStrategy
    {
        bool IsComplete { get; }
        float Cost => 1f;

        public void EnterAction(IContext context)
        {
            // noop
        }

        public void ExecuteAction(IContext context)
        {
            // noop
        }

        public void ExitAction(IContext context)
        {
            // noop
        }
    }

    public class GoapAction
    {
        public string Name { get; private set; }

        public float Cost => staticCost < 0 ? actionStrategy.Cost : staticCost;
        public bool IsComplete => actionStrategy.IsComplete;
        public bool IsInterruptable { get; private set; }

        public List<GoapCondition> Preconditions;
        public List<GoapCondition> Effects;

        private IActionStrategy actionStrategy;
        private float staticCost = -1f;

        private GoapAction()
        {
        }

        private GoapAction(string name)
        {
            Name = name;
            Preconditions = new List<GoapCondition>();
            Effects = new List<GoapCondition>();
        }

        public void Start(IContext context) => actionStrategy.EnterAction(context);

        public void Update(IContext context) => actionStrategy.ExecuteAction(context);

        public void Stop(IContext context) => actionStrategy.ExitAction(context);

        public class Builder
        {
            readonly GoapAction goapAction;

            public Builder(string name)
            {
                goapAction = new GoapAction(name);
            }

            public Builder WithStaticCost(float cost)
            {
                goapAction.staticCost = cost;
                return this;
            }

            public Builder WithStaticCost(bool staticCost, float cost)
            {
                if (staticCost)
                {
                    goapAction.staticCost = cost;
                }

                return this;
            }

            public Builder AddPrecondition(GoapCondition condition)
            {
                goapAction.Preconditions.Add(condition);
                return this;
            }

            public Builder AddPreconditions(List<GoapCondition> conditions)
            {
                if (conditions != null)
                {
                    goapAction.Preconditions = conditions;
                }

                return this;
            }

            public Builder AddEffect(GoapCondition condition)
            {
                goapAction.Effects.Add(condition);
                return this;
            }

            public Builder AddEffects(List<GoapCondition> effects)
            {
                if (effects != null)
                {
                    goapAction.Effects = effects;
                }

                return this;
            }

            public Builder WithStrategy(IActionStrategy strategy)
            {
                goapAction.actionStrategy = strategy;
                return this;
            }

            public Builder IsInterruptable(bool isInterruptable)
            {
                goapAction.IsInterruptable = isInterruptable;
                return this;
            }

            public GoapAction Build()
            {
                return goapAction;
            }
        }
    }
}