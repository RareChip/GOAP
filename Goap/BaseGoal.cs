namespace RareChip.Goap
{
    public interface IGoal
    {
        GoalConfig Config { get; }
        float Insistance { get; }
        void Initialize(GoalConfig config, GoapAgent agent);
    }
        
    public abstract class BaseGoal : IGoal
    {
        public string Name { get; }
        public virtual float Insistance => Config.Insistance;
        public GoalConfig Config { get; private set; }
        public GoapAgent Agent { get; private set; }


        public virtual void Initialize(GoalConfig config, GoapAgent agent)
        {
            this.Config = config;
            this.Agent = agent;
        }

        protected static float WeightInsistance(float value, float start, float end)
        {
            if (value < start)
                return 0;
            if (value > end)
                return 1;
            
            return (value - start) / (end - start);
        } 
    }
        
}
