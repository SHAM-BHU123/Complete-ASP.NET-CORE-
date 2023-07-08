using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly List<Country> _countries;

        //constructor
        public CountriesService(bool initilize = true)
        {
            _countries = new List<Country>();
            if (initilize)
            {
                _countries.AddRange(new List<Country>()
                {
                    new Country()
                {
                    CountryID = Guid.Parse("76B7B242-909A-47FD-A56D-95D1FDC2AD24"),
                    CountryName="USA"
                },
                new Country()
                {
                    CountryID = Guid.Parse("2F73BC0F-2053-4E67-8BAA-B7AFE54C45EA"),
                    CountryName = "Nepal"
                },

                new Country()
                {
                    CountryID = Guid.Parse("912F7D3C-39AC-49D4-8E6A-BA8A601B0A5C"),
                    CountryName = "India"
                },

                new Country()
                {
                    CountryID = Guid.Parse("04751427-DD59-480A-8B25-A119E214CAD5"),
                    CountryName = "China"
                },

                new Country()
                {
                    CountryID = Guid.Parse("DD03041C-4A3D-46C5-B164-527D8BEAE0F5"),
                    CountryName = "Sirlanka"
                },
            });

        }
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
            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into an list of _countries
            _countries.Add(country);

            return country.ToCountryResponse();
        }
        

        
        public List<CountryResponse> GetAllCountryList()
        {
            //Convert all the countries from "Country" type to "CountryResponse" Type.
             return _countries.Select(country=>country.ToCountryResponse()).ToList();
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
            Country? country_response_from_list= _countries.FirstOrDefault(coutries => coutries.CountryID == countryID);

            //Convert matching country form "Country" to "CountryResponse" type && Return CountryResponse object

            if(country_response_from_list == null) 
            { 
                return null;
            }

            return country_response_from_list.ToCountryResponse();

        }
    }
}
