using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using Xunit;
using System.Data.SqlTypes;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testoutputHelper;




        public CountriesServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(false);
            _testoutputHelper = testOutputHelper;
        }


        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() 
            { 
                CountryName = null 
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _testoutputHelper.WriteLine(_countriesService.AddCountry(request1).ToString());
                _testoutputHelper.WriteLine(_countriesService.AddCountry(request2).ToString());
                
            });
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            //Act
            _testoutputHelper.WriteLine("Actual");
            CountryResponse response = _countriesService.AddCountry(request);
            _testoutputHelper.WriteLine(response.ToString());

            _testoutputHelper.WriteLine("Expected");

            List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountryList();

            foreach(CountryResponse c in countries_from_GetAllCountries)
            {
                _testoutputHelper.WriteLine("Country Name:"+c.CountryName+" "+"Country ID:"+c.CountryID);
            }

            //Assert
            Assert.True(response.CountryID != Guid.Empty);

            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default ( before adding any countries)
        public void GetAllCountries_EmptyList()
        {
            //Act 
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountryList();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public void GetAllCountry_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_type = new List<CountryAddRequest>()
            {
                new CountryAddRequest() {CountryName="USA"},
                new CountryAddRequest() {CountryName="Japan"}

            };

            //Act

            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_type)
            {
                countries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
            }

            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountryList();

            //read each element from countries _list_from_add_country

            foreach (CountryResponse expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }

        }

        #endregion

        #region GetCountryByCountryID
        [Fact]

        /*
         * If we supply null as CountryID , it should return null as CountryResponse
        */
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_get_method= 
           _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
           
        }

        [Fact]
/*        If we supply a valid country id, it should return 
 *        matching country details as CountryResponse object
*/        
          public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryResponse country_response_from_add =
                        _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? country_response_from_get =
                     _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

            //Assert

            Assert.Equal(country_response_from_add, country_response_from_get);

        }
        #endregion
    }

}