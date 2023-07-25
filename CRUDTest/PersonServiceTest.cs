using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using Microsoft.Data.SqlClient.Server;
using AutoFixture.Kernel;
using FluentAssertions.Execution;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using CRUDExample.Controllers;
using Serilog;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testoutputHelper;
        private readonly IFixture _fixture;


        /*Represents an mocked object that was created by Mock<T>*/
        private readonly IPersonsRepository _personRepository;

        /*Used to mock the method of IPersonsRepository*/
        private readonly Mock<IPersonsRepository> _personRepositoryMock;

        Mock<ILogger<PersonService>> _loggerMock;

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _loggerMock = new Mock<ILogger<PersonService>>();
            _personRepositoryMock = new Mock<IPersonsRepository>();

             var diagnosticContextMock=new Mock<IDiagnosticContext>();

            _personRepository = _personRepositoryMock.Object;

            _personService = new PersonService(_personRepository,_loggerMock.Object,diagnosticContextMock.Object);

            _testoutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            /*Actually the service method call AddPerson() directly Returns the argument null exception so it
             doesn't call the repository method so that is the region we needn't mock the repository in this test case
             */
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            /*await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _personService.AddPerson(personAddRequest);
            });*/

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
            
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                    .With(temp=>temp.PersonName,null as string).Create();


           Person person= personAddRequest.ToPerson();
         /*When the PersonReposiotry.AddPerson is called , it has to return the same "person" object*/

            _personRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
            
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessfull()
        {
            //Arrange

            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                 .With(temp => temp.Email, "someone@example.com").Create();


            Person person=personAddRequest.ToPerson();

            PersonResponse person_response_expected = person.ConvertPersonToPersonResponse();

            

            /*If we supply any argument value to the AddPerson method , it should return the same return value*/
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

            person_response_expected.PersonID = person_response_from_add.PersonID;

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

             person_response_from_add.Should().Be(person_response_expected);

        }

        #endregion


        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

            //Assert
           /* Assert.Null(person_response_from_get);*/

            person_response_from_get.Should().BeNull();
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async  Task GetPersonByPersonID_WithPersonID_ToBeSucessful()
        {
            //Arange
            /*
             CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
             CountryResponse  country_response = await _countriesService.AddCountry(country_add_request);

            

            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                                                .With(temp=>temp.Email,"someone@example.com").Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);



            PersonResponse? person_response_from_get = 
                await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);

            person_response_from_get.Should().Be(person_response_from_add);
            */

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp=>temp.Country,null as Country).Create();

            PersonResponse person_response_expected= person.ConvertPersonToPersonResponse();

            /*
             First the repository method returns the value to the service method and that service method Returns the same back to test
             */

            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

             PersonResponse ? person_response_from_get=  await _personService.GetPersonByPersonID(person.PersonID);

            person_response_from_get.Should().Be(person_response_expected);
        }

        #endregion


        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            
            List<Person?> person=new List<Person?>();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();
            
            //Assert
           // Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }


        /*First, we will add few persons; and then when we call GetAllPersons(),
         should return the same persons that were added*/

        [Fact]
        public async Task GetAllPersons_AddFewPersons_ToBeSuccessfull()
        {
            //Arrange
            List<Person?> person = new List<Person?>(){
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),
                 };                           

             List<PersonResponse> person_response_list_expected = person.Select(temp => temp.ConvertPersonToPersonResponse()).ToList();
          
            //print person_response_list_from_add
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testoutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);

            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();
          
            //print person_list_from_get
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testoutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            /*  foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }*/
            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons
        /* If the search text is empty and search by is "PersonName" ,
         * it should return all persons
         */

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessfull()
        {
            //Arrange

            List<Person?> person = new List<Person?>(){
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),
                 };

            List<PersonResponse> person_response_list_expected = person
                                      .Select(temp => temp.ConvertPersonToPersonResponse()).ToList();

            


            //print person_response_list_from_add
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testoutputHelper.WriteLine(person_response_from_add.ToString());
            }


                _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(person);

            //Act
            List<PersonResponse> persons_list_from_search = 
                await _personService.GetFilteredPerson(nameof(Person.PersonName), "");

            //print person_list_from_get
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testoutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
           /* foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }*/

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons
        /*First we will add few person ; and then we will search 
         * based on person name with some search
         string.It should return the matching persons
        */

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessfull()
        {
            //Arrange

            List<Person?> person = new List<Person?>(){
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),
                 };

            List<PersonResponse> person_response_list_expected = person
                                      .Select(temp => temp.ConvertPersonToPersonResponse()).ToList();




            //print person_response_list_from_add
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testoutputHelper.WriteLine(person_response_from_add.ToString());
            }


            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(person);

            //Act
            List<PersonResponse> persons_list_from_search =
                await _personService.GetFilteredPerson(nameof(Person.PersonName), "sam");

            //print person_list_from_get
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testoutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            /* foreach (PersonResponse person_response_from_add in person_response_list_from_add)
             {
                 Assert.Contains(person_response_from_add, persons_list_from_search);
             }*/

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPerson
        /*When we sort based on PersonName in DESC ,
         *it should return persons list in descending on Person name 
         */
        [Fact]
        public async Task GetSortedPerson_ToBeSuccessFull()
        {
            //Arrange

            List<Person?> person = new List<Person?>(){
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),

                _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                  .With(temp=>temp.Country,null as Country).
                   Create(),
                 };

            List<PersonResponse> person_response_list_expected = person
                                      .Select(temp => temp.ConvertPersonToPersonResponse()).ToList();


              _personRepositoryMock
                 .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(person);

            //print person_response_list_from_add
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testoutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort =
                await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print person_list_from_get
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testoutputHelper.WriteLine(person_response_from_get.ToString());
            }

            person_response_list_expected = person_response_list_expected
                                           .OrderByDescending(temp => temp.PersonName).ToList();

            /*//Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            *//*{
                Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            }*/

           /* persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);*/
            persons_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);
        }
        #endregion

        #region UpdatePerson_NullPerson
        //When we supply null as PersonUpdateRequest , it should throw ArgumentNullExpection

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest person_update_request = null;



            //Assert
           /*await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
                PersonResponse personResponse = await _personService.UpdatePerson(person_update_request);
            });*/

            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

             await action.Should().ThrowAsync<ArgumentNullException>();
        }
        #endregion


        #region InvalidPersonID
        //if we supply invalid person id it should throw an Argument exception

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest person_update_request = _fixture.Create<PersonUpdateRequest>();

            Person person=person_update_request.ToPerson();

            //Assert
            /* await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 //Act
                 PersonResponse personResponse = await _personService.UpdatePerson(person_update_request);

             });*/

            _personRepositoryMock.Setup(temp => temp.UpdatePerson(person)).ReturnsAsync(person);
            
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();

        }
        #endregion

        #region PersonNameIsNull
        //When the PersonName is null, it should throw Argument Exception

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            

            Person? person = _fixture.Build<Person>()
                              .With(temp=>temp.PersonName,null as string)
                              .With(temp => temp.Email, "someone@example.com")
                              .With(temp=>temp.Country ,null as Country)
                              .With(temp=>temp.Gender,"Male")
                              .Create();

            PersonResponse person_response_from_add = person.ConvertPersonToPersonResponse();

            PersonUpdateRequest person_update_request =
                 person_response_from_add.ToPersonUpdateRequest();



            //Assert
            /*await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });*/

            Func<Task> action  = async () =>
            {
               await _personService.UpdatePerson(person_update_request);
            };

           await action.Should().ThrowAsync<ArgumentException>();

        }
        #endregion

        #region PersonFullDetailsUpdation

        //First , add a new person and try to update name and email

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            

            Person? person = _fixture.Build<Person>()
                                                    .With(temp => temp.Email, "someone@example.com")
                                                    .With(temp=>temp.Gender,"Male")
                                                    .With(temp=>temp.Country , null as Country)
                                                    .Create();

            PersonResponse person_response_expected = person.ConvertPersonToPersonResponse();


            PersonUpdateRequest person_update_request =
                 person_response_expected.ToPersonUpdateRequest();


            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update =
                await _personService.UpdatePerson(person_update_request);


            //Assert
            /*Assert.Equal(person_response_from_get, person_response_from_update);*/

            person_response_from_update.Should().Be(person_response_expected);

        }

        #endregion

        #region DeletePerson
        //if you supply an valid PersonId,it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessfull()
        {
            //Arrange


            Person? person = _fixture.Build<Person>()
                                                .With(temp => temp.Email, "someone@example.com")
                                                .With(temp=>temp.Country, null as Country).Create();

            PersonResponse person_response_from_add = person.ConvertPersonToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()));
            //Act
            bool isDeleted = await _personService.DeletePerson(person.PersonID);
            _testoutputHelper.WriteLine(isDeleted.ToString());
            //Assert
            /*Assert.True(isDeleted);*/

            isDeleted.Should().BeTrue();
        }

        //if you supply an invalid PersonId,it should return false
        [Fact]
        public async Task DeletePerson_InValidPersonID_ToBeSuccessFull()
        {
            //Act
            Person? person = _fixture.Build<Person>()
                                                .With(temp => temp.Email, "someone@example.com")
                                                .With(temp => temp.Country, null as Country).Create();

            PersonResponse person_response_from_add = person.ConvertPersonToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonById(person.PersonID)).ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()));
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());
            _testoutputHelper.WriteLine(isDeleted.ToString());
            //Assert
            /*Assert.False(isDeleted);*/
            isDeleted.Should().BeFalse();
            #endregion

        }
    }
}