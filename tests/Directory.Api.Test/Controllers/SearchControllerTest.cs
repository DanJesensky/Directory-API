using Directory.Api.Controllers;
using Directory.Api.Models;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class SearchControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpMockedDbContext() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Brother.AddRange(new[] {
                new Brother { Id = 1, FirstName = "InactiveFirst", LastName = "InactiveLast", ExpectedGraduation = DateTime.MaxValue },
                new Brother { Id = 2, FirstName = "First", LastName = "Last", ExpectedGraduation = DateTime.MaxValue },
                new Brother { Id = 3, FirstName = "Grad", LastName = "GradLast", ExpectedGraduation = DateTime.MaxValue },
                new Brother { Id = 4, FirstName = "UniqueFirst", LastName = "UniqueLast" },
                new Brother { Id = 5, FirstName = "DuplicatedFirst1", LastName = "DuplicatedLast1" },
                new Brother { Id = 6, FirstName = "DuplicatedFirst2", LastName = "DuplicatedLast2" }
            });

            _dbContext.InactiveBrother.Add(new InactiveBrother { Id = 1, Reason = "Dropped out" });

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void SearchQueryMatchesMultiple_MultipleReturned() {
            SearchController controller = new SearchController(_dbContext);
            OkObjectResult result = controller.Search("Duplicated") as OkObjectResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<MinimalBrother> brothers = result.Value as IQueryable<MinimalBrother>;
                Assert.That(brothers, Is.Not.Null);
                Assert.That(brothers.Count(), Is.EqualTo(2));

                Assert.That(brothers.FirstOrDefault(b => b.Id == 5), Is.Not.Null);
                Assert.That(brothers.FirstOrDefault(b => b.Id == 6), Is.Not.Null);
            });
        }

        [Test]
        // First name only
        [TestCase("InactiveFirst", ExpectedResult = ("InactiveFirst InactiveLast"))]
        [TestCase("Grad", ExpectedResult = "Grad GradLast")]
        [TestCase("q", ExpectedResult = "UniqueFirst UniqueLast")]
        // Last name only
        [TestCase("UniqueLast", ExpectedResult = "UniqueFirst UniqueLast")]
        // Full name
        [TestCase("UniqueFirst UniqueLast", ExpectedResult = "UniqueFirst UniqueLast")]
        // Case insensitivity
        [TestCase("uniquefirst", ExpectedResult = "UniqueFirst UniqueLast")]
        [TestCase("uniquelast", ExpectedResult = "UniqueFirst UniqueLast")]
        // Single search query part searches first and last name
        [TestCase("UniqueF", ExpectedResult = "UniqueFirst UniqueLast")]
        [TestCase("UniqueL", ExpectedResult = "UniqueFirst UniqueLast")]
        // Two search query parts searches the respective first and last name
        [TestCase("UniqueFirst UniqueLast", ExpectedResult = "UniqueFirst UniqueLast")]
        [TestCase("U U", ExpectedResult = "UniqueFirst UniqueLast")]
        // Two parts abbreviated
        [TestCase("UniqueF UniqueL", ExpectedResult = "UniqueFirst UniqueLast")]
        public string SearchQueryForExistingBrother_ReturnsExpectedResult(string query) {
            SearchController controller = new SearchController(_dbContext);
            OkObjectResult result = controller.Search(query) as OkObjectResult;

            IQueryable<MinimalBrother> brothers = result?.Value as IQueryable<MinimalBrother>;
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);

                Assert.That(brothers, Is.Not.Null);
                Assert.That(brothers.Count(), Is.GreaterThan(0));
            });

            MinimalBrother brother = brothers?.FirstOrDefault();
            return $"{brother?.FirstName} {brother?.LastName}";
        }

        [Test]
        public void SearchQueryWithTwoParts_DoesNotSearchAllFirstAndLastNamesForBothParts() {
            SearchController controller = new SearchController(_dbContext);
            OkObjectResult result = controller.Search("UniqueL UniqueF") as OkObjectResult;

            IQueryable<MinimalBrother> brothers = result?.Value as IQueryable<MinimalBrother>;
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);

                Assert.That(brothers, Is.Not.Null);
                Assert.That(brothers.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void SearchQueryContainingNonexistentBrother_ReturnsEmptyList() {
            SearchController controller = new SearchController(_dbContext);
            OkObjectResult result = controller.Search("zzzzzzzzzzz") as OkObjectResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<MinimalBrother> brothers = result.Value as IQueryable<MinimalBrother>;
                Assert.That(brothers, Is.Not.Null);
                Assert.That(brothers.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void EmptyQuery_ReturnsActiveBrothers() {
            SearchController controller = new SearchController(_dbContext);
            OkObjectResult result = controller.Search("") as OkObjectResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);

                IQueryable<MinimalBrother> brothers = result.Value as IQueryable<MinimalBrother>;
                Assert.That(brothers, Is.Not.Null);
                Assert.That(brothers.Count(), Is.GreaterThan(0));
            });
        }
    }
}
