using Directory.Api.Controllers;
using Directory.Data;
using Directory.Test.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class PositionControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            Mock<DirectoryContext> mockContext = new Mock<DirectoryContext>();

            mockContext
                .SetupGet(m => m.Position)
                .Returns(new List<Position> {
                    new Position { Id = 1, Name = "President" }
                }.AsMockedDbSet());

            _dbContext = mockContext.Object;
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
