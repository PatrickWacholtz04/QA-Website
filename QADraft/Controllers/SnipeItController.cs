using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
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

            // Default - return empty model
            List<UserInfo> empty = [];
            return View(empty);
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

            // Check that there was an input, if not, return an empty model
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(id))
            {
                // Default - return empty model
                List<UserInfo> empty = [];
                return View(empty);
            }

            // Pass the form data to the API handler and recieve the response data
            Console.WriteLine($"Name: {name}, Email: {email}, ID: {id}");
            ApiHandler api_handler = new ApiHandler();
            List<UserInfo> result = await api_handler.User_Search(name, email, id, company_id);

            return View(result);

        }

        [HttpGet]
        public IActionResult Select()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Select(string a)
        {
            Console.WriteLine(a);
            return PartialView("Select");
        }
    }
}