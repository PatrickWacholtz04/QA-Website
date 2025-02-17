using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using QADraft.Utilities;


namespace QADraft.Controllers
{
    public class SnipeItSDController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SnipeItHelperSD()
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
        public async Task<IActionResult> SnipeItHelperSD(string name, string email, string id, string company_id)
        {
            if (!SessionUtil.IsAuthenticated(HttpContext))
            {
                return RedirectToAction("Login");
            }

            if (!SessionUtil.CheckPermissions("Coordinator", HttpContext))
            {
                return RedirectToAction("PermissionsDenied", "Home");
            }

            ViewBag.layout = SessionUtil.GetLayout(HttpContext);

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(id))
            {
                return View("SnipeItHelperSD", new List<UserInfo>());
            }

            Console.WriteLine($"Searching - Name: {name}, Email: {email}, ID: {id}");

            ApiHandler api_handler = new ApiHandler();
            List<UserInfo> result = await api_handler.staffFac_Search(name, email, id, company_id);

            return View("SnipeItHelperSD", result);
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