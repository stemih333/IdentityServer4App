using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Quickstart.UI
{
    public class TableLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdministrationViewModel model, string text, string action, int page, string sortColumn)
        {
            return View(new TableLinkViewModel
            {
                Action = action,
                AdminModel = model,
                Page = page,
                Text = text,
                SortColumn = sortColumn
            });
        }
    }
}
