using System;
using Directory.Api.Models;
using Directory.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Directory.Api.Controllers {
    [Route("~/Brother")]
    [ApiController]
    public class BrotherController : ControllerBase {
        private readonly DirectoryContext _dbContext;
        private readonly ClaimsPrincipal _principal;
        private readonly ILogger<BrotherController> _logger;

        public BrotherController(DirectoryContext dbContext, ClaimsPrincipal principal, ILogger<BrotherController> logger) {
            _dbContext = dbContext;
            _principal = principal;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("~/Brother")]
        public IActionResult GetBrothers()
            => Ok(new ContentModel<MinimalBrother>(_dbContext.Brother
                .Where(b => !_dbContext.InactiveBrother.Any(inactive => inactive.Id == b.Id))
                .Where(b => b.ExpectedGraduation > DateTime.Now)
                .OrderByDescending(b => b.ZetaNumber != null)
                    .ThenBy(b => b.ZetaNumber)
                    .ThenBy(b => b.DateJoined)
                    .ThenBy(b => b.LastName)
                    .ThenBy(b => b.FirstName)
                .Select(b => new MinimalBrother { Id = b.Id, FirstName = b.FirstName, LastName = b.LastName, DateJoined = b.DateJoined, ZetaNumber = b.ZetaNumber })));

        [HttpGet]
        [AllowAnonymous]
        [Route("~/Brother/{id}")]
        public async Task<IActionResult> GetBrother(int id) {
            Brother brother = await _dbContext.Brother.FindBrotherByIdAsync(id);

            if (brother == null) {
                return NotFound();
            }

            // Shouldn't send down the picture.
            // This isn't an issue since SaveChanges/SaveChangesAsync aren't called.
            brother.Picture = null;

            return Ok(brother);
        }

        [HttpPost]
        [Authorize]
        [Route("~/Brother/{brotherId}")]
        public async Task<IActionResult> UpdateBrother(int brotherId, [FromBody] Brother newBrotherModel) {
            Brother brother = await _dbContext.Brother.FindBrotherByIdAsync(brotherId);

            if (brother == null) {
                return NotFound();
            }

            if (newBrotherModel.Id != brotherId) {
                return BadRequest();
            }

            string subject = _principal.GetSubjectId();

            if (string.IsNullOrEmpty(subject)) {
                // Well, this shouldn't happen, but just to be sure.
                return Unauthorized();
            }

            IEnumerable<string> scopes = _principal.GetScopes();

            // TODO: I think there's a better way to do this, having the authorization filter do it instead, though this requires several things:
            // (1) the route to be consistent; (2) the parameter to either be obtained from the route in the authorization handler or obtainable
            // there via an injected service; and (3) access to the claims principal.
            // However, doing this would prevent this boilerplate every time. It should also be its own authorization policy so that it doesn't apply everywhere.
            if (!subject.Equals(brother.Id.ToString())) {
                if (!scopes.Contains(Constants.Scopes.Administrator)) {
                    _logger.LogInformation("Rejecting request from user with identifier {subject} attempting to modify {brotherFirst} {brotherLast}: the user is not an administrator.",
                                           subject, brother.FirstName, brother.LastName);
                    return Unauthorized();
                }

                // User is an administrator
                _logger.LogTrace("Administrator (subject {subjectId}) updating {first} {last}.", subject,
                                 brother.FirstName, brother.LastName);
            }

            _dbContext.Entry(brother).CurrentValues.SetValues(newBrotherModel);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}