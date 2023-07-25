using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add the person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Return all persons in the data store
        /// </summary>
        /// <returns>List of person objects from table</returns>
        Task<List<Person?>> GetAllPersons();


        /// <summary>
        /// Returns a person object based on the given person id
        /// </summary>
        /// <param name="personID">PersonID (guid) to search </param>
        /// <returns>A person object or null</returns>
        Task<Person?> GetPersonById(Guid personID);

        /// <summary>
        /// Returns all the person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ Expression to check</param>
        /// <returns>All the matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);


        /// <summary>
        /// Delete an person object based on the person id
        /// </summary>
        /// <param name="personID">Person ID(guid) to Search</param>
        /// <returns>Returns true , if the deletion is successfull ; otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates a person object ( person name and other details ) based on the given person id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
