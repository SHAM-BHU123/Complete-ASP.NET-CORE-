using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;

        }


        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName)!=null)
            {
                throw new ArgumentException("Given country name already exists");
            }



            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into an list of _countries
            await _countriesRepository.AddCountry(country);
            

            return country.ToCountryResponse();
        }



        public async Task<List<CountryResponse>> GetAllCountryList()
        {
            //Convert all the countries from "Country" type to "CountryResponse" Type.
                return (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
            //Return all CountryResponse object
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {


            //Check if "countryID" is not null.
            if (countryID == null)
            {
                return null;
            }
            //Get matching country from List<Country> based CountryID.
            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            //Convert matching country form "Country" to "CountryResponse" type && Return CountryResponse object

            if (country_response_from_list == null)
            {
                return null;
            }

            return country_response_from_list.ToCountryResponse();

        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = worksheet.Dimension.Rows;


                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1]);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if ( await _countriesRepository.GetCountryByCountryName(countryName) ==null)
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                            await _countriesRepository.AddCountry(country);
                            
                            countriesInserted++;
                        }

                    }
                }
            }
            return countriesInserted;
        }

    }
}

