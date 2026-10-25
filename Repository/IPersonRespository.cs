using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    public interface IPersonRespository
    {
        public  Task<Person> AddPerson(Person person);

        public Task<List<Person>> GetAllPersons();

        public Task<Person?> GetPersonByPersonID(Guid? personID);

        public  Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        public Task<bool> DeletePersonByPersonID(Guid? personID);

        public Task<Person> UpdatePerson(Person person);
    }
}
