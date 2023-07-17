using System;
using System.Runtime.CompilerServices;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Person Service
    /// </summary>
    public class PersonResponse
    {
        
        public Guid  PersonID { get; set; }

        public string ? PersonName { get; set; }

        public string? Email { get; set;}

        public DateTime? DateOfBirth { get; set; }

        public string ? Gender { get; set;}

        public Guid? CountryID { get; set; }

        public string? Country { get; set;}

        public string? Address { get; set;}

        public bool? ReceiveNewsLetters { get; set; }

        public double? Age { get; set;}
        /// <summary>
        /// Compare the current object data with the parameter object
        /// </summary>
        /// <param name="obj"> The PersonResponse Object to compare</param>
        /// <returns> True or False , indicating whether all person 
        /// details are matched with the specifed parameter object
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) 
             return false;

            if (obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse personResponse = (PersonResponse)obj;

            return 
            PersonID == personResponse.PersonID &&
            PersonName == personResponse.PersonName &&
            Email == personResponse.Email &&
            DateOfBirth == personResponse.DateOfBirth &&
            Gender == personResponse.Gender &&
            CountryID == personResponse.CountryID &&
            Address == personResponse.Address &&
            ReceiveNewsLetters == personResponse.ReceiveNewsLetters;
            

        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Person ID:{PersonID} ," +
                $"Person Name:{PersonName}," +
                $"Email:{Email}" +
                $",Date of Birth:{DateOfBirth?.ToString("dd mm yyyy")}" +
                $",Gender:{Gender}," +
                $"Country ID:{CountryID}," +
                $"Country:{Country}," +
                $"Address={Address}," + 
                $"Receive News Letters:{ReceiveNewsLetters}";
        }

       public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID= (Guid)PersonID, 
                PersonName=PersonName, 
                Email=Email,
                DateOfBirth=DateOfBirth,
                Gender= (GenderOptions)Enum.Parse(typeof(GenderOptions),Gender,true),
                CountryID=CountryID,
                Address=Address,
                ReceiveNewsLetters=ReceiveNewsLetters


            };
        }


    }

    public static class PersonExtension
    {
        /// <summary>
        /// An extension method to covert an object of Person class to PersonResponse class
        /// </summary>
        /// <param name="person">Return the converted PersonResponse object</param>
        public static PersonResponse ConvertPersonToPersonResponse( this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Address = person.Address,
               
                CountryID = person.CountryID,
                Age = (person.DateOfBirth != null) ?
               
                Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) 
                : null,
                Country = person.Country?.CountryName

            };
        }
       
    }

   
}
