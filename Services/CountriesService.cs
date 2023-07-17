using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly PersonsDbContext _dbContext;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _dbContext = personsDbContext;

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
            if (await _dbContext.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }



            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into an list of _countries
            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            return country.ToCountryResponse();
        }



        public async Task<List<CountryResponse>> GetAllCountryList()
        {
            //Convert all the countries from "Country" type to "CountryResponse" Type.
            return await _dbContext.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
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
            Country? country_response_from_list = await _dbContext.Countries.FirstOrDefaultAsync(coutries => coutries.CountryID == countryID);

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

                        if (_dbContext.Countries.Where(temp => temp.CountryName == countryName).Count() == 0)
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                            _dbContext.Countries.Add(country);
                            await _dbContext.SaveChangesAsync();
                            countriesInserted++;
                        }

                    }
                }
            }
            return countriesInserted;
        }

    }
}

