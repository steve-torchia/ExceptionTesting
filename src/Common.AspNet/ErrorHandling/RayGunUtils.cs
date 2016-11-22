using Microsoft.AspNetCore.Http;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Builders;
using Mindscape.Raygun4Net.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.AspNet
{
    public static class RayGunUtils
    {
        public static async Task<RaygunMessage> BuildRayGunMessageAsync(HttpContext httpContext, Exception ex, IList<string> tags, Dictionary<object, object> customData)
        {
            var msg = new RaygunMessage();

            msg.Details = new RaygunMessageDetails
            {
                User = BuildRayGunIdentifierMessage(httpContext.User),
                //Error = RaygunErrorMessageBuilder.Build(ex),
                Tags = tags,
                UserCustomData = customData,
                Request = await RaygunAspNetCoreRequestMessageBuilder.Build(httpContext, new RaygunRequestMessageOptions())
            };

            return msg;
        }


        public static RaygunIdentifierMessage BuildRayGunIdentifierMessage(ClaimsPrincipal user)
        {
            string email;
            string fullName;
            string uuid;
            bool isAnonomous;

            if (user == null)
            {
                email = "ronald@example.com";
                uuid = "abc123";
                fullName = "Ronald Raygun";
                isAnonomous = true;
            }
            else
            {
                email = user.FindFirst(ClaimTypes.Email)?.Value;
                uuid = user.GetUserObjectId().ToString();
                fullName = user.Identity.Name;
                isAnonomous = false;
            }

            return new RaygunIdentifierMessage(email)
            {
                Email = email,
                UUID = uuid,
                FullName = fullName,
                IsAnonymous = isAnonomous,
            };
        }

    }

}
