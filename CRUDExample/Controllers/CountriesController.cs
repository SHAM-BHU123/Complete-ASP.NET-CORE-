using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {

        private readonly ICountriesService _countriesService;


        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }



        [HttpPost]
        [Route("UploadFromExcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excleFile)
        {
            if (excleFile == null || excleFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View();
            }
            if(!Path.GetExtension(excleFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file format.Please Select xlsx file";
                return View();
            }

            int noOfCountryInserted = await _countriesService.UploadCountriesFromExcelFile(excleFile);
            ViewBag.Message = $"{noOfCountryInserted} Countries Upload";
            return View();
        }
    }
}

