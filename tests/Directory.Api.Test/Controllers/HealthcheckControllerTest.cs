using Directory.Abstractions;
using Directory.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class HealthcheckControllerTest {
        [Test]
        public void HealthCheck_HealthyService_ReturnsOk() {
            Mock<IServiceHealthProvider> healthProvider = new Mock<IServiceHealthProvider>();
            healthProvider.Setup(m => m.IsDatabaseConnected()).Returns(true);

            HealthcheckController controller = new HealthcheckController(healthProvider.Object);
            OkResult result = controller.GetHealthcheck() as OkResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Healthcheck_DatabaseFailure_ReturnsInternalServerError() {
            Mock<IServiceHealthProvider> healthProvider = new Mock<IServiceHealthProvider>();
            healthProvider.Setup(m => m.IsDatabaseConnected()).Returns(false);

            HealthcheckController controller = new HealthcheckController(healthProvider.Object);
            ObjectResult result = controller.GetHealthcheck() as ObjectResult;

            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            });
        }


        [Test]
        public void AuthorizationHealthcheck_ReturnsNoContent() {
            HealthcheckController controller = new HealthcheckController(null);
            NoContentResult result = controller.GetAuthorizationStatus() as NoContentResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}
