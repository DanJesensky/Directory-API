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
    public class QuestionControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            Mock<DirectoryContext> mockContext = new Mock<DirectoryContext>();

            mockContext
                .SetupGet(m => m.Question)
                .Returns(new List<Question> {
                    new Question { Id = 1, QuestionText = "a question" }
                }.AsMockedDbSet());

            _dbContext = mockContext.Object;
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
