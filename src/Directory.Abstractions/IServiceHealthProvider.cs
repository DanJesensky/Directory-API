namespace Directory.Abstractions {
    public interface IServiceHealthProvider {
        bool IsDatabaseConnected();
    }
}
