﻿using Directory.Abstractions;
using Directory.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Directory.Api.Controllers {
    [ApiController]
    public class PictureController : ControllerBase {
        private readonly DirectoryContext _dbContext;
        private readonly IDefaultPictureProvider _defaultPictureProvider;
        private readonly ClaimsPrincipal _principal;
        private readonly ILogger<PictureController> _logger;

        public PictureController(DirectoryContext dbContext, IDefaultPictureProvider defaultPictureProvider, ClaimsPrincipal principal, ILogger<PictureController> logger) {
            _dbContext = dbContext;
            _defaultPictureProvider = defaultPictureProvider;
            _principal = principal;
            _logger = logger;
        }

        /// <summary>
        /// Gets a picture for a given brother.
        /// </summary>
        /// <param name="id">The ID of the brother to fetch the picture for.</param>
        /// <returns>The picture of the brother, as a file (content disposition will be inline).</returns>
        [HttpGet]
        [Route("~/Picture/{id}")]
        public IActionResult GetPicture(int id) {
            Brother brother = _dbContext.Brother.FirstOrDefault(b => b.Id == id);

            if (brother == null) {
                return NotFound();
            }

            if (brother.Picture == null) {
                return GetDefaultPicture();
            }

            // TODO: I think this works for JPEG and PNG images, but need to verify.
            return File(brother.Picture, "image/jpeg");
        }

        [HttpGet]
        [Route("~/Picture/Default")]
        public IActionResult GetDefaultPicture() =>
            File(_defaultPictureProvider.GetDefaultPicture(), MediaTypeNames.Image.Jpeg);

        /// <summary>
        /// Update the picture of a brother.
        /// </summary>
        /// <param name="brotherId">The brother whose picture to replace.</param>
        /// <param name="picture">The picture to use as replacement.</param>
        /// <returns>200 OK with no body if the picture was replaced.</returns>
        [HttpPost]
        [Authorize]
        [Route("~/Picture/{brotherId}")]
        public async Task<IActionResult> ReplacePicture(int brotherId, [FromForm] IFormFile? picture) {
            string? subjectId = _principal.GetSubjectId();
            Brother brother = await _dbContext.Brother.FindBrotherByIdAsync(brotherId);

            if (brother == null) {
                _logger.LogInformation("Received request to modify picture for brother {id} but they do not exist.", brotherId);
                return NotFound();
            }

            _logger.LogInformation("Received request to modify picture for brother {brotherId} ({first} {last}).", brotherId, brother.FirstName, brother.LastName);

            // TODO: The subject id is not necessarily the same as the brother id. They should be linked by a column in the Brother table that does not yet exist.
            IEnumerable<string> scopes = _principal.GetScopes();
            if (!brotherId.ToString().Equals(subjectId, StringComparison.OrdinalIgnoreCase)) {
                if (!scopes.Contains(Constants.Scopes.Administrator)) {
                    _logger.LogTrace("Rejecting request to modify another user's picture from non-administrator user {subject}.", subjectId);
                    return Unauthorized();
                }

                // User is an administrator
                _logger.LogTrace("Administrator replacing picture for {brother}.", brotherId);
            }

            if (picture == null || picture.Length == 0) {
                _logger.LogTrace("Clearing picture.");
                brother.Picture = null;
            } else {
                brother.Picture = new byte[picture.Length];
                await picture.OpenReadStream().ReadAsync(brother.Picture, 0, (int)picture.Length);
            }

            try {
                await _dbContext.SaveChangesAsync();
            } catch (DBConcurrencyException) {
                return Conflict();
            }

            return Ok();
        }
    }
}