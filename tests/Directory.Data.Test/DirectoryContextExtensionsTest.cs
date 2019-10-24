using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Directory.Data.Test {
    [TestFixture]
    public class DirectoryContextExtensionsTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void Setup() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Brother.AddRange(new[] {
                new Brother {Id = 1},
                new Brother {Id = 2}
            });

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
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