using Directory.Api.Models;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Api.Controllers {
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly DirectoryContext _dbContext;

        public QuestionController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns a list of prompts that brothers may have answered.
        /// </summary>
        /// <returns>The list of prompts.</returns>
        [Route("~/Question")]
        public IActionResult GetQuestions() => Ok(new ContentModel<Question>(_dbContext.Question));
    }
}