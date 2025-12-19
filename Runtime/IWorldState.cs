namespace GOAP.Runtime
{
    public interface IWorldState
    {
        public T Get<T>(string key);
    }
}