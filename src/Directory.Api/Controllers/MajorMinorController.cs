using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MajorMinorController : ControllerBase {
        private DirectoryContext _dbContext;

        public MajorMinorController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("~/Major")]
        public IActionResult GetMajors() => Ok(_dbContext.Major);

        [HttpGet]
        [Route("~/Minor")]
        public IActionResult GetMinors() => Ok(_dbContext.Minor);
    }
}