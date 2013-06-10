namespace LazyLog.LogProviders
{
    public delegate void NewDataHandler(string newData);

    public interface ILogProvdier
    {
        event NewDataHandler OnNewData;

        string Description { get; }

        void Start();

        void Stop();

        bool IsRunning { get; }
    }
}
