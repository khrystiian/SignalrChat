using Microsoft.AspNet.SignalR;
using SignalRApp.App_Data;
using SignalrChat.Hubs;
using SignalrChat.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace SignalrChat.Controllers
{
    public class AccountController : Controller
    {
        UserRepo repo = new UserRepo();
        UserHelper helper = new UserHelper();


        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user.Name != null && user.Password != null)
            {
                var usr = repo.GetUser(user.Name);
                if (usr.Name == user.Name && usr.Password == user.Password)
                {
                    FormsAuthentication.SetAuthCookie(usr.Name, false);
                    return RedirectToAction("welcome", "Home", new { usr.Name });
                }
            }
            return View();
        }

        // GET: Account
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user)
        {            
            user.ImagePath = helper.ImageProcessor(user.ImageFile);
            if (user.ImagePath !="")
            {
                user.ImageFile.SaveAs(user.ImagePath);
            }
            bool added = repo.Add(user);
            if (added)
            {
                FormsAuthentication.SetAuthCookie(user.Name, false);
                return RedirectToAction("welcome", "Home", new { user.Name });
            }
            else
            {
                return View();
            }

        }
        public ActionResult Logout(string name)
        {
            var usr = repo.GetUser(name);
            ChatHub.ConnectedUsers.Remove(usr);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

    }
}