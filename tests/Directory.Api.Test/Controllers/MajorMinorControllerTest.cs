using System.Collections;
using System.Collections.Generic;
using Directory.Api.Controllers;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using Directory.Api.Models;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class MajorMinorControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Major.Add(new Major {Id = 1, Name = "Software Engineering"});
            _dbContext.Minor.Add(new Minor { Id = 1, Name = "A minor" });

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetMajors_ReturnsListOfMajors() {
            MajorMinorController controller = new MajorMinorController(_dbContext);
            OkObjectResult result = controller.GetMajors() as OkObjectResult;
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);

                Major[] value = (result.Value as ContentModel<Major>)?.Content.ToArray();
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

                Minor[] value = (result.Value as ContentModel<Minor>)?.Content.ToArray();
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count(), Is.GreaterThan(0));
                Assert.That(value.FirstOrDefault(minor => minor.Id == 1)?.Name,
                            Is.Not.Null.And.EqualTo("A minor"));
            }));
        }
    }
}
