﻿using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Add new person into an  list of the person
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns the same person details , along with newly generated PersonID</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);
        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        List<PersonResponse> GetAllPersons();



        /// <summary>
        /// Returns the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to search</param>
        /// <returns>Return the matching person object</returns>
        PersonResponse ? GetPersonByPersonID(Guid? personID);


        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching person based on
        /// the given search field and search string
        /// </returns>
        List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString);

        /// <summary>
        /// Return the sorted list of the person 
        /// </summary>
        /// <param name="allPersons">Represent the list of the person to sorted</param>
        /// <param name="sortBy">ASC or DESC</param>
        /// <param name="sortOrder"></param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
       List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons,string sortBy,SortOrderOptions sortOrder);
   


        /// <summary>
        /// Updates the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including person id</param>
        /// <returns>Returns the person response object after updation</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a person based on the given person id
        /// </summary>
        /// <param name="PersonID">PersonID to delete</param>
        /// <returns><Returns true , if the deletion is successfull ;otherwise false/returns>
        bool DeletePerson(Guid? personID);
    
    }
}