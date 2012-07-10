using System;
using System.Web.Mvc;

namespace Cheezburger.StarterKit.Controllers
{
    public class AuthorizationController : Controller
    {
        public ActionResult BeginAuthorization()
        {
            return Cheezburger.RedirectToAuthorize();
        }

        public ActionResult CompleteAuthorization(string code, string error, string error_description)
        {
            Session["accessToken"] = Cheezburger.GetAccessToken(code);

            return RedirectToAction("Index", "Home");
        }
    }
}