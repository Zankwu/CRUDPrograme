using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper;
using System.Globalization;
namespace Services
{
    public class PersonService : IPersonService
    {


        private readonly ApplicationDbContext _db;
        public PersonService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest)); 
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            
            if (_db.Persons.Where(item=> item.PersonID == person.PersonID).Count()>0)
            {
                throw new ArgumentException();
            }
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid personID)
        {
            if (personID == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            Person? person_delete = await _db.Persons.FirstOrDefaultAsync(item=>item.PersonID == personID);
            if (person_delete == null)
            {
                return false;
            }
            _db.Persons.Remove(person_delete);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PersonResponse>> GetAllPerson()
        {
            
            
            List<Person> people = await _db.Persons.Include("Country").ToListAsync() ;

            List<PersonResponse> personResponses = _db.sp_GetAllPersons().Select
                (item => item.ToPersonResponse()).ToList();


            return personResponses;
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {

            // 1. 获取所有人员作为基准数据源
            List<PersonResponse> allPersons = await GetAllPerson();
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

        public async Task<PersonResponse> GetPersonByID(Guid personID)
        {
            //throw new NotImplementedException();
            if (personID == Guid.Empty)
            {
                return null;
            }
            Person? personByID = await _db.Persons.FirstOrDefaultAsync(item => item.PersonID == personID);
            return personByID.ToPersonResponse();
        }

        public async Task<MemoryStream> GetPersonCSVFile()
        {
            List<PersonResponse> persons = await GetAllPerson();


            var resultMemory = new MemoryStream();
            var streamWriter = new StreamWriter(resultMemory);
            CsvWriter csv = new CsvWriter
                (streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);
            await csv.WriteRecordsAsync(persons);

            resultMemory.Position = 0;
            return resultMemory;

        }

        public async Task<List<PersonResponse>> GetPersonsByOrder(List<PersonResponse> people, string? orderBy, OrderOptions order)
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException();
            }
            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(item=>item.PersonID == personUpdateRequest.PersonId);

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
            await _db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }
    }
}
