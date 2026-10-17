using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> people =  new List<Person>();

        public PersonResponse AddPerson(PersonAddRequest personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest)); 
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            if (people.Where(item=> item.PersonID == person.PersonID).Count()>0)
            {
                throw new ArgumentException();
            }
            people.Add(person);
            return person.ToPersonResponse();
        }

        public bool DeletePerson(Guid personID)
        {
            if (personID == Guid.Empty)
            {
                throw new ArgumentNullException();
            }

            people.RemoveAll(item=>item.PersonID == personID);

            return people.Where(item=>item.PersonID == personID).Count()>0?false:true;
        }

        public List<PersonResponse> GetAllPerson()
        {
            List<PersonResponse> personResponses = new List<PersonResponse>();
            foreach (var item in people)
            {
                personResponses.Add(item.ToPersonResponse());
            }

            return personResponses;
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {

            // 1. 获取所有人员作为基准数据源
            List<PersonResponse> allPersons = GetAllPerson();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(item=> !string.IsNullOrEmpty(item.PersonName) ?  
                    (item.PersonName.Contains(searchString,StringComparison.OrdinalIgnoreCase)): 
                    true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(item=>!string.IsNullOrEmpty(item.Email)? 
                    (item.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)):
                    true).ToList();
                    break;

                case nameof(PersonResponse.CountryName):
                    matchingPersons = allPersons.Where(item=>!string.IsNullOrEmpty(searchString) ?
                    (item.CountryName.Contains(searchString,StringComparison.OrdinalIgnoreCase)):
                    true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(item => !string.IsNullOrEmpty(searchString) ?
                    item.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) :
                    true).ToList();
                    break;
            }

            return matchingPersons;
        }

        public PersonResponse GetPersonByID(Guid personID)
        {
            //throw new NotImplementedException();
            if (personID == Guid.Empty)
            {
                return null;
            }
            Person personByID = people.FirstOrDefault(item => item.PersonID == personID);
            return personByID.ToPersonResponse();
        }

        public List<PersonResponse> GetPersonsByOrder(List<PersonResponse> people, string? orderBy, OrderOptions order)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return people;
            }

            List < PersonResponse > personResponsesFromSort= new List < PersonResponse >();
            switch (orderBy)
            {
                case nameof(PersonResponse.PersonName):
                    personResponsesFromSort = order == OrderOptions.ASEC ? people.OrderBy(temp=>temp.PersonName).ToList() :
                          people.OrderByDescending(temp => temp.PersonName).ToList();
                    ; break;
            }
            return personResponsesFromSort;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException();
            }
            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = people.FirstOrDefault(item=>item.PersonID == personUpdateRequest.PersonId);
            if (matchingPerson == null)
            {
                throw new ArgumentException();
            }
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString(); // 枚举转字符串
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;

            return matchingPerson.ToPersonResponse();
        }
    }
}
