using Microsoft.AspNet.SignalR;
using SignalRApp.App_Data;
using SignalrChat.Hubs;
using System.Web.Mvc;
using System.Web.Security;

namespace SignalrChat.Controllers
{
    public class HomeController : Controller
    {
        UserRepo repo = new UserRepo();
        UserHelper helper = new UserHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome(string name)
        {
            string path = repo.GetUser(name).ImagePath;
            ViewBag.Image = helper.LoadImage(path);
            FormsAuthentication.SetAuthCookie(name, true);

            return View();
        }

        public ActionResult Chat(string name)
        {
            var currentUsername = User.Identity.Name;
            var currentUser = repo.GetUser(currentUsername); 

            ViewBag.FullName = name;
            ViewBag.Image = helper.LoadImage(currentUser.ImagePath);
            ChatHub.SelectedPeer(name);

            return View();
        }

        public ActionResult About(string name)
        {
            string path = repo.GetUser(name).ImagePath;
            ViewBag.Image = helper.LoadImage(path);

            return View();
        }
    }
}