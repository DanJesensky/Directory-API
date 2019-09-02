using System.Diagnostics.CodeAnalysis;
using Directory.Data;
using Microsoft.EntityFrameworkCore;

namespace Directory.Test.Helpers {
    [ExcludeFromCodeCoverage]
    public class InMemoryDbHelper {
        public static DirectoryContext Context() {
            DbContextOptions<DirectoryContext> opts = new DbContextOptionsBuilder<DirectoryContext>()
                                                      .UseInMemoryDatabase("directory")
                                                      .Options;

            return new DirectoryContext(opts);
        }
    }
}
