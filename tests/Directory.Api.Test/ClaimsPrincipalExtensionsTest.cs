using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using NUnit.Framework;

namespace Directory.Api.Test {
    [TestFixture]
    public class ClaimsPrincipalExtensionsTest {
        [Test]
        [TestCase(JwtClaimTypes.Scope, ExpectedResult = new[] { "scope-a", "scope-b" })]
        [TestCase("SCOPE", ExpectedResult = new[] { "scope-a", "scope-b" })]
        [TestCase("scope1", ExpectedResult = new string[0])]
        public IEnumerable<string> GetScopes_ReturnsCorrectClaims(string claimType) {
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                    new Claim(claimType, "scope-a"),
                    new Claim(claimType, "scope-b")
                }));

            return principal.GetScopes();
        }

        [Test]
        [TestCase(JwtClaimTypes.Subject, ExpectedResult = "1")]
        [TestCase("SUB", ExpectedResult = "1")]
        [TestCase("not-sub", ExpectedResult = null)]
        public string GetSubjectId_ReturnsSubClaimValue(string claimType) {
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                    new Claim(claimType, "1")
                }));

            return principal.GetSubjectId();
        }
    }
}
