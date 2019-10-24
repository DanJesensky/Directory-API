using Directory.Abstractions;
using Directory.Api.Controllers;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class PictureControllerTest {
        private DirectoryContext _dbContext;

        [SetUp]
        public void SetUpDbContextMock() {
            _dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            _dbContext.Database.EnsureCreated();

            _dbContext.Brother.AddRange(new [] {
                new Brother { Id = 1, Picture = new byte[] { 1, 2, 3 } },
                new Brother { Id = 2 }
            });

            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDownDbContext() {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        #region Get Picture

        [Test]
        public void BrotherWithPicture_ReturnsPicture() {
            PictureController controller = new PictureController(_dbContext, null, null, null);
            FileContentResult result = controller.GetPicture(1) as FileContentResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.FileContents, Is.Not.Null);
                Assert.That(result.ContentType, Is.EqualTo("image/jpeg"));
                Assert.That(result.FileContents, Is.EqualTo(new byte[] { 1, 2, 3 }));
            });
        }

        [Test]
        public void BrotherWithoutPicture_ReturnsDefaultPicture() {
            Mock<IDefaultPictureProvider> mockDefaultProvider = new Mock<IDefaultPictureProvider>();
            mockDefaultProvider.Setup(m => m.GetDefaultPicture()).Returns(new byte[] { 3, 2, 1 });
            PictureController controller = new PictureController(_dbContext, mockDefaultProvider.Object, null, null);
            FileContentResult result = controller.GetPicture(2) as FileContentResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.FileContents, Is.Not.Null);
                Assert.That(result.ContentType, Is.EqualTo("image/jpeg"));
                Assert.That(result.FileContents, Is.EqualTo(new byte[] { 3, 2, 1 }));
            });
        }

        [Test]
        public void NonexistentBrother_ReturnsNotFound() {
            PictureController controller = new PictureController(_dbContext, null, null, null);
            NotFoundResult result = controller.GetPicture(-1) as NotFoundResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            });
        }

        #endregion Get Picture

        #region Update Picture

        [Test]
        public async Task ReplacePicture_BrotherDoesNotExist_ReturnsNotFound() {
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                Mock.Of<ClaimsPrincipal>(),
                Mock.Of<ILogger<PictureController>>());

            IActionResult result = await controller.ReplacePicture(-1, null);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            });
        }

        [Test]
        public async Task ReplacePicture_SubjectAndBrotherDifferWithoutAdministratorScope_ReturnsUnauthorized() {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(JwtClaimTypes.Subject, "2") },
                    "Bearer"));
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());

            IActionResult result = await controller.ReplacePicture(1, null);
            
            Assert.Multiple((() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
            }));
        }
        
        [Test]
        public async Task ReplacePicture_SubjectAndBrotherAreSame_ReplacesPicture() {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(JwtClaimTypes.Subject, "1") },
                    "Bearer"));
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());

            byte[] picture = { 4, 5, 6 };
            IFormFile file = new FormFile(new MemoryStream(picture), 0, picture.Length, "file", "file");
            
            IActionResult result = await controller.ReplacePicture(1, file);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<OkResult>());
                Assert.That(_dbContext.Brother.First(b => b.Id.Equals(1)).Picture, Is.EqualTo(picture));
            });
        }

        [Test]
        public async Task ReplacePicture_SubjectAndBrotherDifferWithAdministratorScope_ReplacesPicture() {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                        new Claim(JwtClaimTypes.Subject, "2")
                    },
                    "Bearer"));
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());
            
            byte[] picture = { 7, 8, 9 };
            IFormFile file = new FormFile(new MemoryStream(picture), 0, picture.Length, "file", "file");
            
            IActionResult result = await controller.ReplacePicture(1, file);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<OkResult>());
                Assert.That(_dbContext.Brother.First(b => b.Id.Equals(1)).Picture, Is.EqualTo(picture));
            });
        }

        [Test]
        public async Task ReplacePicture_ZeroLengthFile_ClearsPicture() {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                        new Claim(JwtClaimTypes.Subject, "1")
                    },
                    "Bearer"));
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());
            
            IActionResult result = await controller.ReplacePicture(1, null);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<OkResult>());
                Assert.That(_dbContext.Brother.First(b => b.Id.Equals(1)).Picture, Is.Null);
            });
        }

        [Test]
        public async Task ReplacePicture_NullFile_ClearsPicture() {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                        new Claim(JwtClaimTypes.Subject, "1")
                    },
                    "Bearer"));
            PictureController controller = new PictureController(_dbContext,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());
            
            byte[] picture = { };
            IFormFile file = new FormFile(new MemoryStream(picture), 0, picture.Length, "file", "file");
            
            IActionResult result = await controller.ReplacePicture(1, file);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<OkResult>());
                Assert.That(_dbContext.Brother.First(b => b.Id.Equals(1)).Picture, Is.Null);
            });
        }

        [Test]
        public async Task ReplacePicture_ControllerConcurrencyConflict_ReturnsConflict() {
            await _dbContext.Database.EnsureDeletedAsync();

            using DirectoryContext dbContext = new DirectoryContext(new DbContextOptionsBuilder<DirectoryContext>()
                                              .UseInMemoryDatabase("directory")
                                              .EnableSensitiveDataLogging()
                                              .EnableDetailedErrors()
                                              .Options);
            await dbContext.Database.EnsureCreatedAsync();

            await dbContext.Brother.AddRangeAsync(new[] {
                new Brother { Id = 1, Picture = new byte[] { 1, 2, 3 } },
                new Brother { Id = 2 }
            });

            await dbContext.SaveChangesAsync();

            Mock<DirectoryContext> mockedContext = new Mock<DirectoryContext>();
            mockedContext.SetupGet(m => m.Brother).Returns(dbContext.Brother);
            mockedContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<DBConcurrencyException>();

            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(JwtClaimTypes.Scope, Constants.Scopes.Administrator),
                        new Claim(JwtClaimTypes.Subject, "1")
                    },
                    "Bearer"));
            PictureController controller = new PictureController(mockedContext.Object,
                Mock.Of<IDefaultPictureProvider>(),
                principal,
                Mock.Of<ILogger<PictureController>>());
            
            byte[] picture = { 5 };
            IFormFile file = new FormFile(new MemoryStream(picture), 0, picture.Length, "file", "file");
            
            IActionResult result = await controller.ReplacePicture(1, file);
            
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<ConflictResult>());
                // Make sure the picture is still what it was before
                Assert.That(_dbContext.Brother.First(b => b.Id.Equals(1)).Picture, Is.EqualTo(new byte[]{ 1, 2, 3 }));
            });
        }
        
        #endregion Update Picture
    }
}
