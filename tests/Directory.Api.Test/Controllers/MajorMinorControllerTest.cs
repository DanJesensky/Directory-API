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
    public class MajorMinorControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            Mock<DirectoryContext> mockContext = new Mock<DirectoryContext>();

            mockContext
                .SetupGet(m => m.Major)
                .Returns(new List<Major> {
                    new Major { Id = 1, Name = "Software Engineering" }
                }.AsMockedDbSet());

            mockContext
                .SetupGet(m => m.Minor)
                .Returns(new List<Minor> {
                    new Minor { Id = 1, Name = "A minor" }
                }.AsMockedDbSet());

            _dbContext = mockContext.Object;
        }

        [Test]
        public void GetMajors_ReturnsListOfMajors() {
            MajorMinorController controller = new MajorMinorController(_dbContext);
            OkObjectResult result = controller.GetMajors() as OkObjectResult;
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<Major> value = result.Value as IQueryable<Major>;
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count(), Is.GreaterThan(0));
                Assert.That(value.FirstOrDefault(major => major.Id == 1)?.Name, 
                            Is.Not.Null.And.EqualTo("Software Engineering"));
            }));
        }

        [Test]
        public void GetMinors_ReturnsListOfMinors() {
            MajorMinorController controller = new MajorMinorController(_dbContext);
            OkObjectResult result = controller.GetMinors() as OkObjectResult;
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<Minor> value = result.Value as IQueryable<Minor>;
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count(), Is.GreaterThan(0));
                Assert.That(value.FirstOrDefault(minor => minor.Id == 1)?.Name,
                            Is.Not.Null.And.EqualTo("A minor"));
            }));
        }
    }
}
