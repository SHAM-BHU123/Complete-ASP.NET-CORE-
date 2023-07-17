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

namespace Services
{

    public class PersonService : IPersonsService
    {
        private readonly PersonsDbContext _dbContext;

        private readonly ICountriesService _countriesService;
        public PersonService(PersonsDbContext personsDbContext,ICountriesService countriesService)
        {
            _dbContext = personsDbContext;
            _countriesService = countriesService;
           
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

           /* Then add it into List<Person>
            _dbContex.Persons.Add(person);
            _dbContex.SaveChanges();*/

            await _dbContext.StoreProcedureInsertPerson(person);

            return person.ConvertPersonToPersonResponse(); 
        }

        public async  Task<List<PersonResponse>> GetAllPersons()
        { 
            /*
               Convert all person from "Person" type to "PersonResponse" type.
               Return all PersonResponse object
               Select * From Persons
                return _dbContex.Persons.ToList()
               .Select(temp=>ConvertPersonToPersonResponse(temp)).ToList();
            */

            var person=await _dbContext.Persons.Include("Country").ToListAsync();
            return person.Select(temp=>temp.ConvertPersonToPersonResponse()).ToList();

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
                Person? person = await _dbContext.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);
                if (person == null)
                {
                    return null;
                }
                //Return PersonResponse object.
                return person.ConvertPersonToPersonResponse();
        }

        public async  Task< List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
        {
            //Check if "searchBy" is not null
            List<PersonResponse> allPersons= await GetAllPersons();

            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }
            switch (searchBy)
            {
               case nameof(PersonResponse.PersonName):    
                   matchingPersons = allPersons.Where(temp =>(!string.IsNullOrEmpty(temp.PersonName) ?
                   temp.PersonName.Contains
                   (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                   break;


               case nameof(PersonResponse.Email):
                  matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ?
                  temp.Email.Contains
                  (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                  break;


                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp => ((temp.DateOfBirth!=null ) ?
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains
                    (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                case nameof(PersonResponse.Gender):
                    if (searchString.Equals("Male", StringComparison.OrdinalIgnoreCase))
                    {
                        matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.Gender) && temp.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    else if (searchString.Equals("Female", StringComparison.OrdinalIgnoreCase))
                    {
                        matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.Gender) && temp.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    else
                    {
                        matchingPersons = allPersons;
                    }
                    break;


                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Country) ?
                    temp.Country.Contains
                    (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Address) ?
                    temp.Address.Contains
                    (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                    default:matchingPersons=allPersons;
                    break;
            }
            
            return matchingPersons;
        }

        public  async Task< List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
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

            Person? matchingPerson=await _dbContext.Persons.FirstOrDefaultAsync(temp=>temp.PersonID==personUpdateRequest.PersonID);
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

            await _dbContext.SaveChangesAsync();
            //Convert the person object from "Person" to "PersonResponse" type
            //Return PersonResponse object with update details
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


           Person? person= await _dbContext.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);
            if (person == null)
            {
                return false;
            }
            _dbContext.Persons.Remove(_dbContext.Persons.First(temp => temp.PersonID == personID));
            await _dbContext.SaveChangesAsync();
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

            List<PersonResponse> persons= _dbContext.Persons.Include("Country").Select(temp=>temp.ConvertPersonToPersonResponse()).ToList();
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

                List<PersonResponse> persons = _dbContext.Persons.
                    Include("Country")
                     .Select(temp => temp.ConvertPersonToPersonResponse())
                     .ToList();

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
