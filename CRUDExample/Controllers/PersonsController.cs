using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Collections.Generic;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(
            string searchBy,
            string? searchString,
            string sortBy = nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrderOptions = SortOrderOptions.ASC
            )

        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {

                {nameof(PersonResponse.PersonName), "Person Name"},
                {nameof(PersonResponse.Email), "Email"},
                {nameof(PersonResponse.DateOfBirth), "Date of Birth"},
                {nameof(PersonResponse.Gender), "Gender"},
                {nameof(PersonResponse.CountryID), "Country"},
                {nameof(PersonResponse.Address), "Address"},
            };

            List < PersonResponse > persons  =  await _personsService.GetFilteredPerson(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString=searchString;


            //Sort
            List<PersonResponse> sortedPersons= await _personsService.GetSortedPersons(persons, sortBy, sortOrderOptions);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrderOptions = sortOrderOptions.ToString();

            return View(sortedPersons);
        }



        //Executes when the user clicks on 'Create Person' hyperlink (while opening the create view)
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
           List<CountryResponse>countryResponses = await _countriesService.GetAllCountryList();
             ViewBag.CountryList = countryResponses.Select(temp => new SelectListItem{
                Text=temp.CountryName,
                Value=temp.CountryID.ToString(),
            });
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries =await _countriesService.GetAllCountryList();
                ViewBag.CountryList = countries;
                ViewBag.Errors= ModelState.Values.SelectMany(value=>value.Errors).
                Select(error=>error.ErrorMessage).ToList();   
                return View();
                
            }
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public async Task<IActionResult> Edit(Guid personID)
        {
           PersonResponse? personResponse= await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest=  personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countryResponses = await _countriesService.GetAllCountryList();
            ViewBag.CountryList = countryResponses.Select(temp => new SelectListItem
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString(),
            });
            return View(personUpdateRequest);
        }


        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
          PersonResponse? personResponse= await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

         if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
               PersonResponse updatePersonResponse =await _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");

            }

            else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountryList();
                ViewBag.CountryList = countries;
                return View(personResponse.ToPersonUpdateRequest());
            }
        }


        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.PersonName = personResponse.PersonName;
            return View(personResponse);
        }


        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

             await _personsService.DeletePerson(personUpdateRequest.PersonID);
           
             return RedirectToAction("Index");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //get the list of persons
            List<PersonResponse> persons= await _personsService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonPDF", persons, ViewData)
            {
              PageMargins=new Rotativa.AspNetCore.Options.Margins()
              {
                  Top=20,Right=20,Bottom=20,Left=20
              },

             PageOrientation=Rotativa.AspNetCore.Options.Orientation.Portrait
            };

        }


        [Route("PersonCSV")]
        public async Task<IActionResult> PersonCSV()
        {
            MemoryStream memoryStream=await _personsService.GetPersonCSV();
            return File(memoryStream, "application/octet", "persons.csv");
        }



        [Route("PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
