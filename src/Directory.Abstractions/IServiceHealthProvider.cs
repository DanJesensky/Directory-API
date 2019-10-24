namespace Directory.Abstractions {
    public interface IServiceHealthProvider {
        /// <summary>
        /// Checks if the database can be reached.
        /// </summary>
        /// <returns>True if the database responds, false otherwise</returns>
        bool IsDatabaseConnected();
    }
}
