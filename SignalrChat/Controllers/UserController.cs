using SignalRApp.App_Data;
using SignalrChat.Models;
using System.Web.Mvc;

namespace SignalrChat.Controllers
{
    public class UserController : Controller
    {
        UserRepo repo = new UserRepo();
        UserHelper helper = new UserHelper();

        // GET: User/Edit/name
        public ActionResult Edit(string name)
        {
            string path = repo.GetUser(name).ImagePath;
            ViewBag.Image = helper.LoadImage(path);
            return View(repo.FindUserByID(name));
        }

        // POST: User/Edit/user
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (user.Password == user.ConfirmPassword)
            {
                user.ImagePath = helper.ImageProcessor(user.ImageFile);
                if (user.ImagePath != "")
                {
                    user.ImageFile.SaveAs(user.ImagePath);
                }
                repo.UpdateUser(user);

                return RedirectToAction("welcome", "Home", new { user.Name });
            }
            else
            {
                return View();
            }
        }

    }
}
