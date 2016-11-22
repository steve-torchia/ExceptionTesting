using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mindscape.Raygun4Net;
using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net.Builders;
using Mindscape.Raygun4Net.Messages;
using System.Collections.Generic;

namespace Common.AspNet
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IRaygunAspNetCoreClientProvider _rgProvider;

        public CustomExceptionFilterAttribute(ILoggerFactory loggerFactory, IRaygunAspNetCoreClientProvider rgProvider)
        {
            _rgProvider = rgProvider;
        }

        public async override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Result = new JsonResult(new ExceptionResponse
            {
                ErrorId = Guid.NewGuid(),
                Message = exception.Message
            });

            var msg = await RayGunUtils.BuildRayGunMessageAsync(
                context.HttpContext,
                exception,
                new[] { "TagABC", "TAGXYZ" },
                new Dictionary<object, object> { { "keykey", "valval" } }
                );


            var rgClient = _rgProvider.GetClient(new RaygunSettings(), context.HttpContext);
            await rgClient.SendInBackground(msg);
        }
    }

    public class ExceptionResponse
    {
        public Guid ErrorId { get; set; }
        public string Message { get; set; }
    }
}