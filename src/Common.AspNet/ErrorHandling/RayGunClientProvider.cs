using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

namespace Common.AspNet
{
    public class RayGunClientProvider : DefaultRaygunAspNetCoreClientProvider
    {
        public const string UnhandledExceptionTag = "Unhandled";

        private readonly string _apiKey;
        
        public RayGunClientProvider(IOptions<RayGunConfig> config)
        {
            _apiKey = config.Value.ApiKey;
        }

        public RayGunClientProvider(string apiKey)
        {
            _apiKey = apiKey;
        }

        public override RaygunClient GetClient(RaygunSettings settings)
        //public override RaygunClient GetClient(RaygunSettings settings, HttpContext context)
        {
            settings.ApiKey = _apiKey;
            
            var client = base.GetClient(settings);

            client.SendingMessage += (sender, args) =>
            {
                if (args.Message.Details.User == null)
                {
                    // Possbily an unhandled exception because we "should" be supplying user data when we handle errors
                    // todo: figure out a way to get the user information from the context here
                    // NOTE:  when RG 5.3.2 gets its bug fixed we can get user info from the HTTP Context
                    // related: https://raygun.com/forums/thread/65336#65335 &  https://raygun.com/forums/thread/65407
                    args.Message.Details.User = new RaygunIdentifierMessage(UnhandledExceptionTag)
                    {
                        UUID = "abc123",
                        Email = "ronald@example.com",
                        FullName = "Ronald Raygun",
                    };

                    args.Message.Details.Tags = new[] { UnhandledExceptionTag };

                }

                // If you want to cancel the message based on some decision:
                // args.Cancel = true;
            };

       
            // strip out all cookie info:
            client.IgnoreCookieNames("*");

            // strip addl fields that may have sensitive data in it
            client.IgnoreServerVariableNames("ALL_HTTP", "ALL_RAW", "HTTP_COOKIE");

            // strip out certain headers
            client.IgnoreHeaderNames("Authorization", "Cookie");

            // ignore password field on the form
            client.IgnoreFormFieldNames("*password*");

            return client;
        }
    }
}
