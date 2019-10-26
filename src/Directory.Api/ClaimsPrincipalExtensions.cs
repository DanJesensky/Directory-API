using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Directory.Api {
    public static class ClaimsPrincipalExtensions {
        /// <summary>
        /// Gets the subject id from the claims principal.
        /// </summary>
        /// <param name="this">The claims principal.</param>
        /// <returns>The subject id, or null if none exists.</returns>
        public static string? GetSubjectId(this ClaimsPrincipal @this)
            => @this.Claims
                    .FirstOrDefault(claim => claim.Type.Equals(JwtClaimTypes.Subject, StringComparison.OrdinalIgnoreCase))?
                    .Value;

        /// <summary>
        /// Gets the list of scopes that the principal was authorized to access at the time of security token evaluation.
        /// </summary>
        /// <param name="this">The claims principal.</param>
        /// <returns>The list of scopes the principal is authorized for.</returns>
        public static IEnumerable<string> GetScopes(this ClaimsPrincipal @this)
            => @this.Claims
                    .Where(claim => claim.Type.Equals(JwtClaimTypes.Scope, StringComparison.OrdinalIgnoreCase))
                    .Select(claim => claim.Value);
    }
}
