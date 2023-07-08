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
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);


        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All the countries from the list as List of CountryResponse</returns>
        List<CountryResponse> GetAllCountryList();



        /// <summary>
        /// Returns a country object based on the given country id
        /// </summary>
        /// <param name="countryID">CountryID(guid) to search</param>
        /// <returns>Matching country as CountryResponse</returns>
       CountryResponse? GetCountryByCountryID(Guid? countryID);

         
    }
}