using Directory.Abstractions;
using Directory.Data;

namespace Directory {
    public class ServiceHealthProvider : IServiceHealthProvider {
        private readonly DirectoryContext _dbContext;

        public ServiceHealthProvider(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        public bool IsDatabaseConnected() => _dbContext?.Database?.EnsureCreated() ?? false;
    }
}
