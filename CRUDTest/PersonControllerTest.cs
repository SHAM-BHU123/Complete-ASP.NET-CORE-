﻿using AutoFixture;
using Castle.Core.Logging;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTest
{
    public class PersonControllerTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countryService;

        private readonly Mock<IPersonsService> _personServiceMock;
        private readonly Mock<ICountriesService> _countryServiceMock;

        private readonly Fixture _fixture;

        Mock<ILogger<PersonsController>> _loggerMock;


        public PersonControllerTest()
        {
           _fixture=new Fixture();
            _personServiceMock=new Mock<IPersonsService>();
            _countryServiceMock=new Mock<ICountriesService>();

            _countryService=_countryServiceMock.Object;
            _personService= _personServiceMock.Object;
            _loggerMock = new Mock<ILogger<PersonsController>>();

        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewsWithPersonsList()
        {
            //Arrange
            List<PersonResponse> person_response_list=_fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personService,_countryService,_loggerMock.Object);

             _personServiceMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(person_response_list);


            _personServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(),It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(person_response_list);

            //Act
              IActionResult result = await personsController.Index(_fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>());


            //Assert
             ViewResult viewResult =Assert.IsType<ViewResult>(result);
             
             viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();

             viewResult.ViewData.Model.Should().Be(person_response_list);
        }
        #endregion

        #region Create
        [Fact]
        public async void Create_IfModelErrors_ToReturnsCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countryServiceMock
             .Setup(temp => temp.GetAllCountryList())
             .ReturnsAsync(countries);

            _personServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countryService, _loggerMock.Object);


            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

            viewResult.ViewData.Model.Should().Be(person_add_request);

        }

        [Fact]
        public async void Create_IfNoModelErrors_ToReturnsRedirectToIndexView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countryServiceMock
             .Setup(temp => temp.GetAllCountryList())
             .ReturnsAsync(countries);

            _personServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countryService, _loggerMock.Object);


            //Act
            

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");

        }
        #endregion
    }
}
