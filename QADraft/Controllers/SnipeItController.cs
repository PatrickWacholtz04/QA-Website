using Microsoft.AspNetCore.Mvc;
using QADraft.Utilities;


namespace QADraft.Controllers
{
    public class SnipeItController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search()
        {

            Console.Write("on load");

            // Verify that the user is logged in, if not direct them to login page
            if (!SessionUtil.IsAuthenticated(HttpContext))
            {
                return RedirectToAction("Login");
            }

            // Verify that the user has the permissions to view this page
            if (!SessionUtil.CheckPermissions("Coordinator", HttpContext))
            {
                return RedirectToAction("PermissionsDenied", "Home");
            }

            // Assign the appropriate layout
            ViewBag.layout = SessionUtil.GetLayout(HttpContext);

            // If the user is not logged in (most likely case), return the login page
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string name, string email, string id, string company_id)
        {
            // Verify that the user is logged in, if not direct them to login page
            if (!SessionUtil.IsAuthenticated(HttpContext))
            {
                return RedirectToAction("Login");
            }

            // Verify that the user has the permissions to view this page
            if (!SessionUtil.CheckPermissions("Coordinator", HttpContext))
            {
                return RedirectToAction("PermissionsDenied", "Home");
            }

            // Assign the appropriate layout
            ViewBag.layout = SessionUtil.GetLayout(HttpContext);


            Console.WriteLine($"Name: {name}, Email: {email}, ID: {id}");
            ApiHandler api_handler = new ApiHandler();
            List<UserInfo> test = await api_handler.User_Search(name, email, id, company_id);

            foreach (var user in test)
            {
                Console.WriteLine(user.Email);
            }


            return View();
        }


    }
}