namespace cron.net.Utils.Serialization
{
    public interface ISerializer<T>
    {
        T Get();
        void Set(T item);
    }
}