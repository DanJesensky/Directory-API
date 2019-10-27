using System;
using Directory.Api.Models;
using Directory.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Directory.Api.Controllers {
    [ApiController]
    public class SearchController : ControllerBase {
        private readonly DirectoryContext _dbContext;

        public SearchController(DirectoryContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Searches the database for brothers that match the search query. At this time, only names are searched.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A list of brothers that match the given query. Always returns OK, even if the list is empty.</returns>
        [HttpGet]
        [Route("~/Search/{query}")]
        public IActionResult Search(string query) {
            string[] queries = query.Split(" ");

            // Discern which where clause to use
            Expression<Func<Brother, bool>> predicate;
            if (queries.Length == 2) {
                // If there are exactly two parts to the query, we assume it's "First Last".
                predicate = b
                    => b.FirstName.Contains(queries[0], StringComparison.OrdinalIgnoreCase)
                    && b.LastName.Contains(queries[1], StringComparison.OrdinalIgnoreCase);
            } else {
                // If there is only one part, we look in both the first and last name for a match.
                // Additionally, if someone puts in 3 or more criteria, we treat it the same way, because it's impossible to tell
                // if they meant to search for "First Middle Last" or something else (especially when middle names are not stored).
                predicate = b
                    => queries.Any(q => b.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase))
                    || queries.Any(q => b.LastName.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            IEnumerable<MinimalBrother> matches =
                _dbContext
                .Brother
                .Where(predicate)
                .Select(b =>
                    new MinimalBrother {
                        Id = b.Id,
                        FirstName = b.FirstName,
                        LastName = b.LastName
                    });

            return Ok(new ContentModel<MinimalBrother>(matches));
        }
    }
}