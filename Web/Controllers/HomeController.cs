using System.Web.Mvc;

namespace Cheezburger.StarterKit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var accessToken = Session["accessToken"] as AccessToken;
            if (accessToken == null)
                return RedirectToAction("BeginAuthorization", "Authorization");

            return View(Cheezburger.GetCurrentUser(accessToken));
        }
    }
}
