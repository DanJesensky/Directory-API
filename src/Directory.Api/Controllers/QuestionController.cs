using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private DirectoryContext _dbContext;

        public QuestionController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        public IActionResult GetQuestions() => Ok(_dbContext.Question);
    }
}