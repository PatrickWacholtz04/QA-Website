using Microsoft.AspNetCore.Mvc;

namespace QADraft.Views.SnipeIt.ViewComponents
{
    public class SelectViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
