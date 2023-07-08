using System;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService method
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        /*
         * It returns the current object to another object of CountryResponse type and return true, if both
           values are same otherwise false
        */

        public override string ToString()
        {
            return $"Country ID:{CountryID} ," +
                $"Country ID: {CountryName}";
        }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            CountryResponse country_to_compare = (CountryResponse)obj;

             return this.CountryID == country_to_compare.CountryID
             && this.CountryName == country_to_compare.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class CountryExtension
    {
         /*
         * Convert from Country object to CountryResponse object
         */
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }
}
