using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase {
        private DirectoryContext _dbContext;

        public PositionController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("~/Position")]
        public IActionResult GetPositions() => Ok(_dbContext.Position);
    }
}