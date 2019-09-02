using Directory.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {

    public class HealthcheckController : ControllerBase {
        private IServiceHealthProvider _healthProvider;

        public HealthcheckController(IServiceHealthProvider healthProvider = null) {
            _healthProvider = healthProvider;
        }

        [HttpGet("/healthcheck")]
        public IActionResult GetHealthCheck() => Ok();

        [Authorize]
        [HttpGet("/authorization-check")]
        public IActionResult GetAuthorizationStatus() => NoContent();
    }
}
