using Directory.Api.Controllers;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class PositionControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Position.Add(new Position {Id = 1, Name = "President"});

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetPositions_ReturnsListOfPositions() {
            PositionController controller = new PositionController(_dbContext);
            OkObjectResult result = controller.GetPositions() as OkObjectResult;
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<Position> value = result.Value as IQueryable<Position>;
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count(), Is.GreaterThan(0));
                Assert.That(value.FirstOrDefault(position => position.Id == 1), Is.Not.Null);
            }));
        }
    }
}
