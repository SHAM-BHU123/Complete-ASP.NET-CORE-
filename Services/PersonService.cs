using System;
using ServiceContracts;
using Entities;
 
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Net.Http.Headers;
using ServiceContracts.Enums;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;

namespace Services
{

    public class PersonService : IPersonsService
    {
        private readonly IPersonsRepository _personRepository;
        private readonly ILogger<PersonService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonService(IPersonsRepository personsRepository,ILogger<PersonService> logger,IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
           
         }

        /* private  PersonResponse ConvertPersonToPersonResponse(Person person)
         {
             //Return PersonResponse object with generated PersonID
             PersonResponse personResponse = person.ConvertPersonToPersonResponse();

             personResponse.Country = _countriesService.
             GetCountryByCountryID(person.CountryID)?.CountryName;
             return personResponse;
         }*/

        public async Task< PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
           //Check if "personAddRequest" is not null
            if(personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }
            /*
            if(string.IsNullOrEmpty(personAddRequest.PersonName))
            {
                throw new ArgumentException("Person name cann't be empty");
            }
            */

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);


            //Convert "personAddRequest" from "PersonAddRequest" type to "Person"
            Person person = personAddRequest.ToPerson();

           //Generate new PersonID.
           person.PersonID=Guid.NewGuid();

            /* Then add it into List<Person>*/
             await _personRepository.AddPerson(person);
            

            /*await _dbContext.StoreProcedureInsertPerson(person);*/

            return person.ConvertPersonToPersonResponse(); 
        }

        public async  Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            /*
               Convert all person from "Person" type to "PersonResponse" type.
               Return all PersonResponse object
               Select * From Persons
                return _dbContex.Persons.ToList()
               .Select(temp=>ConvertPersonToPersonResponse(temp)).ToList();
            */

            /* var person=await _personRepository.Persons.Include("Country").ToListAsync();
             return person.Select(temp=>temp.ConvertPersonToPersonResponse()).ToList();*/

                List<Person?> allPersonList = await _personRepository.GetAllPersons();

            return allPersonList
           .Select(temp => temp.ConvertPersonToPersonResponse()).ToList();

            /*return _dbContext.StoredProcedureGetAllPersons()
           .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();*/

            /*var persons = await _dbContext.StoredProcedureGetAllPersons().ToListAsync();
            return persons.Select(temp => ConvertPersonToPersonResponse(temp)).ToList();*/

        }


        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            //check if "personID" is not null.
            if (personID == null)
            {
                return null;
            }
            //Get matching person from List<Person> based PersonID.
            //Convert matching person object from "Person" to "PersonResponse" type.
            Person? person = await _personRepository.GetPersonById(personID.Value);
                if (person == null)
                {
                    return null;
                }
                //Return PersonResponse object.
                return person.ConvertPersonToPersonResponse();
        }

        public async  Task< List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");
            //Check if "searchBy" is not null
            /*List<PersonResponse> allPersons= await GetAllPersons();

            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }*/
            List<Person> persons = null;
            using (Operation.Time("Time for Filtered Persons from Database"))
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                     await _personRepository.GetFilteredPersons(temp =>
                     temp.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                     await _personRepository.GetFilteredPersons(temp =>
                     temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                    await _personRepository.GetFilteredPersons(temp =>
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),



                    nameof(PersonResponse.Gender) =>
                    await _personRepository.GetFilteredPersons(temp =>
                    temp.Gender.ToLower() == searchString.ToLower()),

                    nameof(PersonResponse.CountryID) =>
                     await _personRepository.GetFilteredPersons(temp =>
                     temp.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                    await _personRepository.GetFilteredPersons(temp =>
                    temp.Address.Contains(searchString)),

                    _ => await _personRepository.GetAllPersons()
                };
            }//end of "using block" of serilog timing
            _diagnosticContext.Set("Persons", persons);
            return persons.Select(temp => temp.ConvertPersonToPersonResponse()).ToList();
        }

        public  async Task< List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation(" of PersonService");
            if (string.IsNullOrEmpty(sortBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons =  (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName),SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp=> temp.PersonName,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.PersonName,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),
                
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),


                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Age).ToList(),


                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Gender,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Gender,
                StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Country,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Country,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Address,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Address,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _=>allPersons

            };
            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //Check if"personUpdateRequest" is not null

            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(Person));
            }

            //Validate all the properties of "PersonUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);

            /*this is a stored procedure code
            Person person = personUpdateRequest.ToPerson();
            _dbContext.StoreProcedureUpdatePerson(person);
            return ConvertPersonToPersonResponse(person);*/

            //Get all the matching "Person" object form List<Person> based on PersonID

            Person? matchingPerson = await _personRepository.GetPersonById(personUpdateRequest.PersonID);


            if(matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            //Convert the person object from "Person" to "PersonResponse" type
            //Return PersonResponse object with update details
             await _personRepository.UpdatePerson(matchingPerson);
             return matchingPerson.ConvertPersonToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException();
            }
            /*Guid personid = (Guid)personID;
            \
            if (personid.Equals(Guid.Empty))
            {
                return false;
            }*/
            //_dbContext.StoreProcedureDeletePerson(personid);


             Person ? person= await _personRepository.GetPersonById(personID.Value);
            if (person == null)
            {
                return false;
            }
            await _personRepository.DeletePersonByPersonID(personID.Value);
            
            return true;
        }

        public async Task<MemoryStream> GetPersonCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter=new StreamWriter(memoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter=new CsvWriter(streamWriter, csvConfiguration);
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();

            List<PersonResponse> persons= await GetAllPersons();
            foreach(PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            await csvWriter.WriteRecordsAsync(persons);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonSheet");
                worksheet.Cells["A1"].Value="Person Name";
                worksheet.Cells["B1"].Value="Email";
                worksheet.Cells["C1"].Value="Date of Birth";
                worksheet.Cells["D1"].Value="Age";
                worksheet.Cells["E1"].Value="Gender";
                worksheet.Cells["F1"].Value="Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                using(ExcelRange excelRange = worksheet.Cells["A1:H1"])
                {
                    excelRange.Style.Fill.PatternType=OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    excelRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;

                List<PersonResponse> persons = await GetAllPersons();

                foreach(PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                    {
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    }
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;
                    row++;
              
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;

       }
    }
}
