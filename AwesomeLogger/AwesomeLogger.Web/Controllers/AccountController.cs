using System.Web.Mvc;
using System.Web.Security;
using AwesomeLogger.Web.Constants;
using AwesomeLogger.Web.Infrastructure.Configuration;
using AwesomeLogger.Web.Models;

namespace AwesomeLogger.Web.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        private readonly IConfigurationProvider _config;

        public AccountController(IConfigurationProvider config)
        {
            _config = config;
        }

        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToRoute(RouteNames.HomeMvc);
            }

            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return View(model);
        }

        private bool ValidateUser(string username, string password)
        {
            if (username != _config.Get(SettingNames.AdminUsername) ||
                password != _config.Get(SettingNames.AdminPassword))
            {
                return false;
            }

            return true;
        }

        [Route("logoff")]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute(RouteNames.HomeMvc);
        }
    }
}