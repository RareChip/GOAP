namespace GOAP.goap
{
    public interface IActionStrategy
    {
        public void Start();
        public void Execute();
        public void Stop();
    }
}