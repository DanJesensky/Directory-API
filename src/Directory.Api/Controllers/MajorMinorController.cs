﻿using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MajorMinorController : ControllerBase {
        private readonly DirectoryContext _dbContext;

        public MajorMinorController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns a list of major courses of study.
        /// </summary>
        /// <returns>The list of majors that exist in the system.</returns>
        [HttpGet]
        [Route("~/Major")]
        public IActionResult GetMajors() => Ok(_dbContext.Major);

        /// <summary>
        /// Returns a list of minor courses of study.
        /// </summary>
        /// <returns>The list of minors that exist in the system.</returns>
        [HttpGet]
        [Route("~/Minor")]
        public IActionResult GetMinors() => Ok(_dbContext.Minor);
    }
}