using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Directory.Api {
    public static class ClaimsPrincipalExtensions {
        public static string GetSubjectId(this ClaimsPrincipal @this)
            => @this.Claims
                    .FirstOrDefault(claim => claim.Type.Equals(JwtClaimTypes.Subject, StringComparison.OrdinalIgnoreCase))?
                    .Value;

        public static IEnumerable<string> GetScopes(this ClaimsPrincipal @this)
            => @this.Claims
                    .Where(claim => claim.Type.Equals(JwtClaimTypes.Scope, StringComparison.OrdinalIgnoreCase))
                    .Select(claim => claim.Value);
    }
}
