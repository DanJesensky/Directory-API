using Directory.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {

    public class HealthcheckController : ControllerBase {
        private readonly IServiceHealthProvider _healthProvider;

        public HealthcheckController(IServiceHealthProvider healthProvider) {
            _healthProvider = healthProvider;
        }

        /// <summary>
        /// Checks that the service is "healthy".
        /// </summary>
        /// <returns>Ok if the service is healthy, 500 Internal Server Error if there's an issue.</returns>
        [HttpGet("/healthcheck")]
        public IActionResult GetHealthcheck() {
            if (!_healthProvider.IsDatabaseConnected()) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to connect to database.");
            }

            return Ok();
        }

        /// <summary>
        /// Diagnostic endpoint to check that a principal is authorized.
        /// </summary>
        /// <returns>204 No Content if it got this far (AuthorizeFilter would catch it and return 401/403 if not).</returns>
        [Authorize]
        [HttpGet("/authorization-check")]
        public IActionResult GetAuthorizationStatus() => NoContent();
    }
}
