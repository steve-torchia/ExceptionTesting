using System;
using System.Security.Claims;

namespace Common
{
    public static class ClaimsExtensions
    {
        public static string ToMicrosoftIdentityClaimsUrl(this string claimsType)
        {
            return $"http://schemas.microsoft.com/identity/claims/{claimsType.ToLowerInvariant()}";
        }


        public static Guid GetUserObjectId(this ClaimsPrincipal user)
        {
            var val = user.FindFirst("objectidentifier".ToMicrosoftIdentityClaimsUrl())?.Value;
            Guid guid;
            Guid.TryParse(val, out guid);
            return guid;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
