using Directory.Api.Controllers;
using Directory.Api.Models;
using Directory.Data;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class BrotherControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void CreateDbContext() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                .UseInMemoryDatabase("directory")
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .Options);
            _dbContext.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        #region Tests for GET /Brother

        [Test]
        public void InactiveBrother_IsNotInListOfActiveBrothers() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last", ExpectedGraduation = DateTime.MaxValue }).Entity.Id;
            _dbContext.InactiveBrother.Add(new InactiveBrother { Id = id, Reason = "dropped out" });
            _dbContext.SaveChanges();

            BrotherController controller = new BrotherController(_dbContext, null, null);
            ContentModel<MinimalBrother> result =
                (controller.GetBrothers() as OkObjectResult)?.Value as ContentModel<MinimalBrother>;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Content.Count(), Is.EqualTo(0));
                Assert.That(result.Content.All(b => b.Id != id));
            });
        }

        [Test]
        public void ActiveBrother_IsInListOfActiveBrothers() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last", ExpectedGraduation = DateTime.MaxValue }).Entity.Id;
            _dbContext.SaveChanges();

            BrotherController controller = new BrotherController(_dbContext, null, null);
            ContentModel<MinimalBrother> result =
                (controller.GetBrothers() as OkObjectResult)?.Value as ContentModel<MinimalBrother>;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Content.Count(), Is.EqualTo(1));
                Assert.That(result.Content.Any(b => b.Id == id));
            });
        }

        [Test]
        public void GraduatedBrothers_AreNotReturnedFromList() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last", ExpectedGraduation = new DateTime(1900, 1, 1) }).Entity.Id;
            _dbContext.SaveChanges();

            BrotherController controller = new BrotherController(_dbContext, null, null);

            Assert.Multiple(() => {
                OkObjectResult result = controller.GetBrothers() as OkObjectResult;
                Assert.That(result, Is.Not.Null);

                IEnumerable<MinimalBrother> brothers = (result.Value as ContentModel<MinimalBrother>)?.Content;
                Assert.That(brothers, Is.Not.Null);

                Assert.That(brothers.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void GetBrothers_NoExpectedGraduation_Excluded() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last" }).Entity.Id;
            _dbContext.SaveChanges();

            BrotherController controller = new BrotherController(_dbContext, null, null);

            Assert.Multiple(() => {
                OkObjectResult result = controller.GetBrothers() as OkObjectResult;
                Assert.That(result, Is.Not.Null);

                IEnumerable<MinimalBrother> brothers = (result.Value as ContentModel<MinimalBrother>)?.Content;
                Assert.That(brothers, Is.Not.Null);

                Assert.That(brothers.Count(), Is.EqualTo(0));
            });
        }

        /// <summary>
        /// The ordering is prioritized as follows:
        /// Brothers with a zeta number first, sorted by it, oldest first.
        /// Any that do not have a zeta number are ordered by date joined, oldest first.
        /// If there are multiple with the same join date, they are ordered by last name, then first, both alphabetically.
        /// </summary>
        [Test]
        public async Task ActiveBrothers_AreInCorrectOrder() {
            await _dbContext.Database.EnsureDeletedAsync();
            _dbContext.SaveChanges();

            DateTime sameDate = DateTime.Now;
            List<Brother> brotherList = new List<Brother> {
                // Should be 1 (Zeta number)
                new Brother { FirstName = "FName", LastName = "LName", ExpectedGraduation = DateTime.MaxValue, ZetaNumber = 1 },
                // Should be 2 (Zeta number)
                new Brother { FirstName = "FName1", LastName = "LName1", ExpectedGraduation = DateTime.MaxValue, ZetaNumber = 2 },
                // Should be 3 (Join date)
                new Brother { FirstName = "FName2", LastName = "LName2", ExpectedGraduation = DateTime.MaxValue, DateJoined = sameDate.AddDays(-5) },
                // Should be 4 (Join date)
                new Brother { FirstName = "FName3", LastName = "LName3", ExpectedGraduation = DateTime.MaxValue, DateJoined = sameDate.AddDays(-3) },
                // Should be 5 (Last name)
                new Brother { FirstName = "ZFirst", LastName = "ALast", ExpectedGraduation = DateTime.MaxValue, DateJoined = sameDate },
                // Should be 6 (First name)
                new Brother { FirstName = "AFirst", LastName = "ZLast", ExpectedGraduation = DateTime.MaxValue, DateJoined = sameDate },
                // Should be 7 (First name)
                new Brother { FirstName = "ZFirst", LastName = "ZLast", ExpectedGraduation = DateTime.MaxValue, DateJoined = sameDate }
            };

            DirectoryContext dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                    .UseInMemoryDatabase("directory")
                    .Options);
            await dbContext.Brother.AddRangeAsync(brotherList);
            await dbContext.SaveChangesAsync();

            BrotherController controller = new BrotherController(dbContext, new ClaimsPrincipal(), Mock.Of<ILogger<BrotherController>>());
            Assert.Multiple(() => {
                OkObjectResult result = controller.GetBrothers() as OkObjectResult;
                Assert.That(result, Is.Not.Null);

                IEnumerable<MinimalBrother> brothers = (result.Value as ContentModel<MinimalBrother>)?.Content;
                Assert.That(brothers, Is.Not.Null);

                Assert.That(brothers.Count(), Is.EqualTo(7));

                for (int i = 0; i < brotherList.Count; i++) {
                    MinimalBrother actual = brothers.ElementAt(i);
                    Brother expected = brotherList[i];

                    Assert.That(actual.LastName, Is.EqualTo(expected.LastName));
                    Assert.That(actual.FirstName, Is.EqualTo(expected.FirstName));
                    Assert.That(actual.ZetaNumber, Is.EqualTo(expected.ZetaNumber));
                    Assert.That(actual.DateJoined, Is.EqualTo(expected.DateJoined));
                }
            });
        }

        #endregion Tests for GET /Brother

        #region Tests for GET /Brother/Id

        [Test]
        public async Task ActiveBrother_IsReturned() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last" }).Entity.Id;
            await _dbContext.SaveChangesAsync();

            BrotherController controller = new BrotherController(_dbContext, null, null);
            Brother result = ((OkObjectResult)await controller.GetBrother(id)).Value as Brother;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(id));
                Assert.That(result.FirstName, Is.EqualTo("First"));
                Assert.That(result.LastName, Is.EqualTo("Last"));
            });
        }

        [Test]
        public async Task InactiveBrother_IsReturned() {
            int id = _dbContext.Brother.Add(new Brother { FirstName = "First", LastName = "Last" }).Entity.Id;
            _dbContext.InactiveBrother.Add(new InactiveBrother { Id = id, Reason = "dropped out" });
            await _dbContext.SaveChangesAsync();

            BrotherController controller = new BrotherController(_dbContext, null, null);
            Brother result = ((OkObjectResult)await controller.GetBrother(id)).Value as Brother;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(id));
                Assert.That(result.FirstName, Is.EqualTo("First"));
                Assert.That(result.LastName, Is.EqualTo("Last"));
            });
        }

        [Test]
        public async Task NonexistentBrother_ReturnsNotFound() {
            BrotherController controller = new BrotherController(_dbContext, null, null);
            IActionResult result = await controller.GetBrother(-1);

            Assert.Multiple(() => {
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
                Assert.That((result as NotFoundResult)?.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }

        #endregion Tests for GET /Brother/Id

        #region Tests for POST /Brother/Id

        [Test]
        public async Task UpdateBrother_WithNonexistentId_Returns404() {
            BrotherController controller = new BrotherController(_dbContext, null, null);
            Assert.That(await controller.UpdateBrother(-1, new Brother { Id = 1 }), Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateBrother_WithoutSubject_IsUnauthorized() {
            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            _dbContext.SaveChanges();

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Scope, "some-scope")
            }));

            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());
            Assert.That(await controller.UpdateBrother(id, new Brother { Id = id }), Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdateBrother_WithNonAdministratorDifferentPrincipal_IsUnauthorized() {
            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            _dbContext.SaveChanges();

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Subject, "some-subject"),
                new Claim(JwtClaimTypes.Scope, "directory")
            }));

            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());
            Assert.That(await controller.UpdateBrother(id, new Brother { Id = id }), Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdateBrother_WithMismatchingIds_ReturnsBadRequest() {
            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            _dbContext.SaveChanges();

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                new Claim(JwtClaimTypes.Subject, "some-subject"),
                new Claim(JwtClaimTypes.Scope, "directory")
            }));

            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());
            Assert.That(await controller.UpdateBrother(id, new Brother { Id = -1 }), Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateBrother_ChangesArePersisted() {
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                new Claim(JwtClaimTypes.Subject, "some-subject"),
                new Claim(JwtClaimTypes.Scope, "directory")
            }));

            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            await _dbContext.SaveChangesAsync();
            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());

            Brother brother = new Brother {
                Id = id,
                FirstName = "firstname1",
                LastName = "lastname1"
            };

            OkResult result = await controller.UpdateBrother(id, brother) as OkResult;
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);
                Brother changed = _dbContext.Brother.FirstOrDefault(b => b.Id == id);
                Assert.That(changed, Is.Not.Null);
                Assert.That(changed.FirstName, Is.EqualTo("firstname1"));
                Assert.That(changed.LastName, Is.EqualTo("lastname1"));
            }));
        }

        [Test]
        public async Task UpdateBrother_DifferentPrincipalAdministrator_IsSuccessful() {
            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            await _dbContext.SaveChangesAsync();

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Subject, "some-subject"),
                new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator)
            }));

            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());
            Assert.That(await controller.UpdateBrother(id, new Brother { Id = id }), Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task UpdateBrother_SamePrincipalNonAdministrator_IsSuccessful() {
            int id = _dbContext.Brother.Add(new Brother()).Entity.Id;
            await _dbContext.SaveChangesAsync();

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(JwtClaimTypes.Subject, id.ToString()),
                new Claim(JwtClaimTypes.Scope, "directory")
            }));
            BrotherController controller = new BrotherController(_dbContext, principal, Mock.Of<ILogger<BrotherController>>());
            IActionResult result = await controller.UpdateBrother(id, new Brother { Id = id });

            Assert.Multiple(() => { Assert.That(result, Is.TypeOf<OkResult>()); });
        }

        #endregion Tests for POST /Brother/Id
    }
}
