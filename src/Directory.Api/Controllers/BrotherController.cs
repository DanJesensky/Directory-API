using Directory.Api.Models;
using Directory.Data;
using Directory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        /// <summary>
        /// Gets the list of brothers for the grid on the homepage/index. This filters out inactive and graduated members.
        /// Members are ordered by initiated members first, then by initiation order, then date joined, then names (last then first).
        /// </summary>
        /// <returns>The list of brothers, in order, to display in the grid.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("~/Brother")]
        public IActionResult GetBrothers() =>
            Ok(new ContentModel<MinimalBrother>(_dbContext.Brother
                // Filter out inactive brothers
                .Where(b => !_dbContext.InactiveBrother.Any(inactive => inactive.Id == b.Id))
                // Filter out graduated brothers
                .Where(b => b.ExpectedGraduation > DateTime.Now)
                // Order by initiated brothers first
                .OrderByDescending(b => b.ZetaNumber != null)
                    // Then by initiation number descending (oldest first)
                    .ThenBy(b => b.ZetaNumber)
                    // Then by date joined for uninitiated members
                    .ThenBy(b => b.DateJoined)
                    // Then by last name for those that joined on the same date
                    .ThenBy(b => b.LastName)
                    // Then by first name for those that have the same name (which wouldn't be a first).
                    .ThenBy(b => b.FirstName)
                // We only care about ID, name, date joined, and initiation number for the grid.
                .Select(b => new MinimalBrother { Id = b.Id, FirstName = b.FirstName, LastName = b.LastName, DateJoined = b.DateJoined, ZetaNumber = b.ZetaNumber })));

        /// <summary>
        /// Gets information about a specific brother.
        /// </summary>
        /// <param name="id">The unique ID of the brother to fetch.</param>
        /// <returns>Available information about the brother, but not their picture.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("~/Brother/{id}")]
        public async Task<IActionResult> GetBrother(int id) {
            BrotherDetailModel brother =
                _dbContext.Brother
                    .Include(b => b.BrotherPosition)
                        .ThenInclude(position => position.Position)
                    .Include(b => b.BrotherMajor)
                        .ThenInclude(major => major.Major)
                    .Include(b => b.BrotherMinor)
                        .ThenInclude(minor => minor.Minor)
                    .Include(b => b.Answer)
                        .ThenInclude(answer => answer.Question)
                    .Select(b => b.ToBrotherDetailModel())
                    .FirstOrDefault(b => b.Id == id);

            if (brother == null) {
                return NotFound();
            }

            // Add the little brothers and big brother. Could do this via EFCore for the big brother with some effort,
            // but not the little brothers. It can't do the query in the reverse as far as I can tell.
            brother.BigBrother = _dbContext
                .Brother
                .Include(b => b.InactiveBrother)
                .FirstOrDefault(big => big.Id == _dbContext.Brother.First(b => b.Id == brother.Id).BigBrotherId)?
                .ToRelatedBrotherModel();

            brother.LittleBrothers = _dbContext
                .Brother
                .Include(b => b.InactiveBrother)
                .Where(b => b.BigBrotherId == brother.Id)
                .Select(b => b.ToRelatedBrotherModel());

            return Ok(brother);
        }

        /// <summary>
        /// Replaces data members of a brother model in the database.
        /// </summary>
        /// <param name="brotherId">The ID of the brother to update. This must be the same as the ID in the model.</param>
        /// <param name="newBrotherModel">The model with data to use to update the brother.</param>
        /// <returns>Empty OK response if the brother was updated successfully.</returns>
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

            string? subject = _principal.GetSubjectId();

            if (string.IsNullOrEmpty(subject)) {
                // Well, this shouldn't happen, but just to be sure.
                return Unauthorized();
            }

            IEnumerable<string> scopes = _principal.GetScopes();

            // TODO: I think there's a better way to do this (and the bit above), having the authorization filter do it instead, though this requires several things:
            // (1) the route to be consistent; (2) the parameter to either be obtained from the route in the authorization handler or obtainable
            // there via an injected service; and (3) access to the claims principal.
            // However, doing this would prevent this boilerplate every time. It should also be its own authorization policy so that it doesn't apply everywhere.
            if (!subject.Equals(brother.Id.ToString())) {
                // If the user isn't the brother they're trying to update and doesn't have the administrator scope, reject the request.
                if (!scopes.Contains(Constants.Scopes.Administrator)) {
                    _logger.LogInformation("Rejecting request from user with identifier {subject} attempting to modify {brotherFirst} {brotherLast}: the user is not an administrator.",
                                           subject, brother.FirstName, brother.LastName);
                    return Unauthorized();
                }

                // User is an administrator
                _logger.LogTrace("Administrator (subject {subjectId}) updating {first} {last}.", subject,
                                 brother.FirstName, brother.LastName);
            }

            try {
                _dbContext.Entry(brother).CurrentValues.SetValues(newBrotherModel);
            } catch (DbUpdateConcurrencyException) {
                return Conflict();
            }

            return Ok();
        }

        /// <summary>
        /// Returns a list of brothers with minimal information (only id and full name).
        ///
        /// Note that this includes all brothers, not just active ones.
        /// </summary>
        /// <returns>A list of all brothers with only their names and IDs.</returns>
        [HttpGet]
        [Route("~/Brother/Minimal")]
        public IActionResult GetMinimalBrothers() =>
            Ok(new ContentModel<IdNameModel>(_dbContext.Brother.Select(b => new IdNameModel { Id = b.Id, Name = $"{b.FirstName} {b.LastName}" })));
    }
}