using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Common.AspNet;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;
using Mindscape.Raygun4Net.Builders;
using System.Security.Claims;
using Common;

namespace ExceptionTesting.Controllers
{
   // [ServiceFilter(typeof(CustomExceptionFilterAttribute))]
    public class HomeController : Controller
    {
        IRaygunAspNetCoreClientProvider _rgProvider;

        public HomeController(IRaygunAspNetCoreClientProvider rgProvider)
        {
            _rgProvider = rgProvider;
        }

        //public IActionResult Index()
        //{
        //    throw new Exception("Yikes an Error!!!");
        //    //return View();
        //}

        public async Task<IActionResult> Index()
        {
            try
            {
                throw new Exception("Yikes an Error!!!");
                //return View();
            }
            catch (Exception ex)
            {
                var msg = new RaygunMessage();

                msg.Details = new RaygunMessageDetails
                {
                    User = RayGunUtils.BuildRayGunIdentifierMessage(HttpContext.User),
                    Error = RaygunErrorMessageBuilder.Build(ex),
                    Request = await RaygunAspNetCoreRequestMessageBuilder.Build(HttpContext, new RaygunRequestMessageOptions())
                };

                var rgClient = _rgProvider.GetClient(new RaygunSettings(), HttpContext);
                await rgClient.SendInBackground(msg);

                return View();
                //return new JsonResult(new ExceptionResponse
                //{
                //    ErrorId = Guid.NewGuid(),
                //    Message = ex.Message
                //}); ;
            }
        }



        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
