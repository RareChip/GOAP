namespace GOAP.Runtime
{
    public interface IWorldState
    {
        public T Get<T>(string key);
        public void Update<T>(string key, T value);
        public IWorldState Clone();
        public bool Equals(object other);
        public int GetHashCode();
    }
}