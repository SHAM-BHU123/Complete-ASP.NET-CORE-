using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represent business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add the country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">
        /// Country object to add
        /// </param>
        /// <returns>Return the country object after adding it (including newly generated country id)</returns>
       Task< CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);


        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All the countries from the list as List of CountryResponse</returns>
         Task<List<CountryResponse>> GetAllCountryList();



        /// <summary>
        /// Returns a country object based on the given country id
        /// </summary>
        /// <param name="countryID">CountryID(guid) to search</param>
        /// <returns>Matching country as CountryResponse</returns>
       Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);




        /// <summary>
        /// Upload countries from the excel file into database
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns>Returns the no of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);

         
    }
}