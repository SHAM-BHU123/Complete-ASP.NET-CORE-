using Microsoft.AspNetCore.Mvc;
using ViewComponentExample.Models;

namespace ViewComponentExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }
        
        [Route("friends-list")]
        public IActionResult LoadFriendsList()
        {
            PersonGridModel personGridModel = new PersonGridModel()
            {
                GridTitle = "Persons List",
                Person = new List<Person>() {
                    new Person() { PersonName="Shambhu",JobTitle="Software Developer"},
                    new Person() { PersonName="Sagar",JobTitle="Teacher"}
                }
            };

            return ViewComponent("Grid", new { grid=personGridModel });
        }


    }
}
