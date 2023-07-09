using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly PersonsDbContext dbContext;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            dbContext = personsDbContext;
             
        }
    


        

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            
            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (dbContext.Countries.Count(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into an list of _countries
            dbContext.Countries.Add(country);
            dbContext.SaveChanges();

            return country.ToCountryResponse();
        }
        

        
        public List<CountryResponse> GetAllCountryList()
        {
            //Convert all the countries from "Country" type to "CountryResponse" Type.
             return dbContext.Countries.Select(country=>country.ToCountryResponse()).ToList();
            //Return all CountryResponse object
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
           

            //Check if "countryID" is not null.
            if(countryID == null)
            {
               return null;
            }
            //Get matching country from List<Country> based CountryID.
            Country? country_response_from_list= dbContext.Countries.FirstOrDefault(coutries => coutries.CountryID == countryID);

            //Convert matching country form "Country" to "CountryResponse" type && Return CountryResponse object

            if(country_response_from_list == null) 
            { 
                return null;
            }

            return country_response_from_list.ToCountryResponse();

        }
    }
}
