using GOAP.Runtime;

namespace GOAP.Testing.ActionStrategies
{
    public class NoOpStrategy : IActionStrategy
    {
        public void Start()
        {
            // Noop
        }

        public void Execute()
        {
            // Noop
        }

        public void Stop()
        {
            // Noop
        }
    }
}