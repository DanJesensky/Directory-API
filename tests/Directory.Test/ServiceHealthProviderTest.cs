using Directory.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Directory.Test {
    [TestFixture]
    public class ServiceHealthProviderTest {
        [Test]
        [TestCase(true, ExpectedResult = true)]
        [TestCase(false, ExpectedResult = false)]
        public bool IsDatabaseConnected_ChecksDatabaseExistence(bool dbFacadeEnsureCreatedResult) {
            Mock<DirectoryContext> dbCtx = new Mock<DirectoryContext>();

            Mock<DatabaseFacade> facade = new Mock<DatabaseFacade>(new object[] { dbCtx.Object });
            facade.Setup(m => m.EnsureCreated()).Returns(dbFacadeEnsureCreatedResult);

            dbCtx.Setup(m => m.Database).Returns(facade.Object);

            ServiceHealthProvider provider = new ServiceHealthProvider(dbCtx.Object);

            return provider.IsDatabaseConnected();
        }
    }
}
