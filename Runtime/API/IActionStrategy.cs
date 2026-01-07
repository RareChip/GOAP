namespace GOAP.Runtime.API
{
    public interface IActionStrategy
    {
        public bool IsComplete { get; }
        public void Start();
        public void Execute();
        public void Stop();
    }
}