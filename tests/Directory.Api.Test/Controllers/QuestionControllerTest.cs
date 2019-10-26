using Directory.Api.Controllers;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class QuestionControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Question.Add(new Question {Id = 1, QuestionText = "a question"});

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetQuestions_ReturnsListOfQuestions() {
            QuestionController controller = new QuestionController(_dbContext);
            OkObjectResult result = controller.GetQuestions() as OkObjectResult;

            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<Question> value = result.Value as IQueryable<Question>;
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count(), Is.GreaterThan(0));
                Assert.That(value.FirstOrDefault(position => position.Id == 1), Is.Not.Null);
            }));
        }
    }
}
