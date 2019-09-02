using Directory.Test.Helpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Data.Test {
    [TestFixture]
    public class DirectoryContextExtensionsTest {
        private DirectoryContext _dbContext;
        [SetUp]
        public void Setup() {
            Mock<DirectoryContext> contextMock = new Mock<DirectoryContext>();

            contextMock
                .SetupGet(m => m.Brother)
                .Returns(new List<Brother> {
                    new Brother { Id = 1 },
                    new Brother { Id = 2 }
                }.AsMockedDbSet());

            _dbContext = contextMock.Object;
        }

        [Test]
        public async Task FindBrotherById_ReturnsCorrectBrother() {
            Brother b = await _dbContext.Brother.FindBrotherByIdAsync(2);

            Assert.Multiple(() => {
                Assert.That(b, Is.Not.Null);
                Assert.That(b.Id, Is.EqualTo(2));
            });
        }
    }
}