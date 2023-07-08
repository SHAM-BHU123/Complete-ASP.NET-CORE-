using System;
using ServiceContracts;
using Entities;
 
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Net.Http.Headers;
using ServiceContracts.Enums;
using System.Runtime.ConstrainedExecution;

namespace Services
{

    public class PersonService : IPersonsService
    {
        private readonly List<Person> _persons;

        private readonly ICountriesService _countriesService;
        public PersonService(bool initilize=true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
            if (initilize)
            {
                _persons.Add(new Person()
                {
                    PersonID=Guid.Parse(" B17A63F5-4816-444A-A65C-6885DEB505E4"),
                    PersonName="Shambhu Pandey",
                    Email="shambhu123@gmail.com",
                    DateOfBirth=DateTime.Parse("1999-02-2"),
                    Gender="Male",
                    Address="Wasinton",
                    ReceiveNewsLetters=false,
                    CountryID=Guid.Parse("76B7B242-909A-47FD-A56D-95D1FDC2AD24")
                    
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("83E1A614-582C-45D5-A67D-F3C6A9445D0E"),
                    PersonName = "John",
                    Email = "john123@gmail.com",
                    DateOfBirth = DateTime.Parse("1989-02-2"),
                    Gender = "Male",
                    Address = "Gulmi",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("2F73BC0F-2053-4E67-8BAA-B7AFE54C45EA")

                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("81D821F1-7604-44B5-9346-4474E46075DA"),
                    PersonName = "Smith",
                    Email = "smith123@gmail.com",
                    DateOfBirth = DateTime.Parse("2002-02-2"),
                    Gender = "Male",
                    Address = "Mumbai",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("912F7D3C-39AC-49D4-8E6A-BA8A601B0A5C")

                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("06D0D479-8CE4-4E86-86EA-1DB9CF6A1355"),
                    PersonName = "Larry",
                    Email = "larryl123@gmail.com",
                    DateOfBirth = DateTime.Parse("1996-02-2"),
                    Gender = "Female",
                    Address = "Colombo",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("04751427-DD59-480A-8B25-A119E214CAD5")

                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("06D0D479-8CE4-4E86-86EA-1DB9CF6A1355"),
                    PersonName = "Juli",
                    Email = "Juli123@gmail.com",
                    DateOfBirth = DateTime.Parse("2005-02-2"),
                    Gender = "Female",
                    Address = "Lanka",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("DD03041C-4A3D-46C5-B164-527D8BEAE0F5")

                });
                
            }

        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            //Return PersonResponse object with generated PersonID
            PersonResponse personResponse = person.ConvertPersonToPersonResponse();

            personResponse.Country = _countriesService.
            GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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

           //Then add it into List<Person>
           _persons.Add(person);

            return ConvertPersonToPersonResponse(person);  
        }

        public List<PersonResponse> GetAllPersons()
        {
            //Convert all person from "Person" type to "PersonResponse" type.
            //Return all PersonResponse object
            return _persons.Select(temp=>ConvertPersonToPersonResponse(temp)).ToList();
        }


        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            //check if "personID" is not null.
            if (personID == null)
            {
                return null;
            }
                //Get matching person from List<Person> based PersonID.
                //Convert matching person object from "Person" to "PersonResponse" type.
                Person? person =_persons.FirstOrDefault(temp => temp.PersonID == personID);
                if (person == null)
                {
                    return null;
                }
                //Return PersonResponse object.
                return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString)
        {
            //Check if "searchBy" is not null
            List<PersonResponse> allPersons=GetAllPersons();

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

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //Check if"personUpdateRequest" is not null

            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(Person));
            }

            //Validate all the properties of "PersonUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);


            //Get all the matching "Person" object form List<Person> based on PersonID

           Person ? matchingPerson =
                _persons.FirstOrDefault(temp=>temp.PersonID==personUpdateRequest.PersonID);

            //Check if matching "Person" object in not null
            if(matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exit");
            }
            //Update all details from "PersonUpdateRequest" object to "Person" object

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender= personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            //Convert the person object from "Person" to "PersonResponse" type
            //Return PersonResponse object with update details
            return ConvertPersonToPersonResponse(matchingPerson); 
             
            

        }

        public bool DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException();
            }
            Person? person=_persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null)
            {
                return false;
            }
            _persons.RemoveAll(temp => temp.PersonID == personID);
            return true;
        }
    }
}
