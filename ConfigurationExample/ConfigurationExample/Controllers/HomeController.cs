using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherApiOption _option;

        public HomeController(IOptions<WeatherApiOption> weatherApiOptions)
        {
           _option  = weatherApiOptions.Value;
        }
        [Route("/")]
        public IActionResult Index()
        {
            ViewBag.ClientID = _option.ClientID;
            ViewBag.ClientSecret= _option.ClientSecret;
;           
            return View();
        }
    }
}
