#region Usings

using System.Web.Mvc;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.Controllers
{
    public class HomeController : Controller
    {
        #region Ctor

        public HomeController()
        {
        }

        public HomeController(IAppSettings settings)
        {
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }
    }
}