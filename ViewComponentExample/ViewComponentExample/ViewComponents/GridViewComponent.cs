using Microsoft.AspNetCore.Mvc;
using ViewComponentExample.Models;

namespace ViewComponentExample.ViewComponents
{
    public class GridViewComponent : ViewComponent
    {
     public async  Task<IViewComponentResult> InvokeAsync(PersonGridModel grid)
        {
           
             return View("Default",grid); 
            // this view searches the partial location invoked a partial view Views/Shared/Component/Grid/Default.cshtml     
        }
    }
}
