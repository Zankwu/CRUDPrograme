using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryImpl
{
    public class PersonRepository : IPersonRespository
    {
        private readonly ApplicationDbContext _db;
        private ILogger<PersonRepository> _logger;
        public PersonRepository(ApplicationDbContext db, ILogger<PersonRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid? personID)
        {
            List<Person> people = await _db.Persons.Where(item => item.PersonID == personID).ToListAsync();
            _db.Persons.RemoveRange(people);

            int rows = await _db.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("[PersonRepository]method in repository");
            List<Person> people = await _db.Persons.Include("Country").ToListAsync();

            return people;
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons
                                .Where(predicate)        // ★ 直接传表达式，EF 会翻成 SQL 的 WHERE
                                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid? personID)
        {
            Person? people = await _db.Persons.FirstOrDefaultAsync(item => item.PersonID == personID);
            return people;

        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons
        .FirstOrDefaultAsync(p => p.PersonID == person.PersonID);

            // 理论上服务层已校验过，这里是防御性兜底
            if (matchingPerson == null)
                return person;

            // 逐字段覆盖
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

            // ★ 故意不复制 PersonID —— 主键不可变

            await _db.SaveChangesAsync();
            return matchingPerson;
        }
    }
}
