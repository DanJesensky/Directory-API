using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase {
        private readonly DirectoryContext _dbContext;

        public PositionController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns a list of positions within the chapter.
        /// </summary>
        /// <returns>The list of active positions.</returns>
        [HttpGet]
        [Route("~/Position")]
        public IActionResult GetPositions() => Ok(_dbContext.Position);
    }
}