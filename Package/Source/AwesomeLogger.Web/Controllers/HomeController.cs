using System.Web.Mvc;
using AwesomeLogger.Web.Constants;

namespace AwesomeLogger.Web.Controllers
{
    [Authorize]
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        [Route("{id:int}")]
        [Route("new")]
        [Route("audit/{*url}")]
        [Route(Name = RouteNames.HomeMvc)]
        public ActionResult Index()
        {
            return View();
        }
    }
}