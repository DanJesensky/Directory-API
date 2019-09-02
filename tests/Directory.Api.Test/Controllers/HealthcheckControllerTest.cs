using Directory.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Directory.Api.Test.Controllers {
    [TestFixture]
    public class HealthcheckControllerTest {
        [Test]
        public void Healthcheck_ReturnsOk() {
            HealthcheckController controller = new HealthcheckController(null);
            OkResult result = controller.GetHealthCheck() as OkResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AuthorizationHealthcheck_ReturnsNoContent() {
            HealthcheckController controller = new HealthcheckController(null);
            NoContentResult result = controller.GetAuthorizationStatus() as NoContentResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}
