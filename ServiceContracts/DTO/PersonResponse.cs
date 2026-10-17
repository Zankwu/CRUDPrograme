using Entities;

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public int? Age { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }
        public string? Email { get; set; }
        public Guid? CountryID { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null && obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }
            PersonResponse other = obj as PersonResponse;
            return other.PersonName == PersonName && PersonID == other.PersonID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"Person ID: {PersonID}, " +
                $"Person Name: {PersonName}, " +
                $"Email: {Email}, " +
                $"Date of Birth: {DateOfBirth?.ToString("dd MMM yyyy")}, " +
                $"Gender: {Gender}, " +
                $"Country ID: {CountryID}, " +
                $"Country: {CountryName}, " +
                $" Age: {Age}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = PersonID,
                CountryID = CountryID,
                PersonName = PersonName,
                Age = (DateOfBirth != null) ?
                DateTime.Today.Year - DateOfBirth.Value.Year -
                (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0) : null,

            };
        }
    }
        public static class PersonExtendMethod
        {
            public static PersonResponse ToPersonResponse(this Person person)
            {
                return new PersonResponse()
                {
                    PersonID = person.PersonID,
                    CountryID = person.CountryID,
                    PersonName = person.PersonName,
                    Age = (person.DateOfBirth != null) ?
                    DateTime.Today.Year - person.DateOfBirth.Value.Year -
                    (DateTime.Today.DayOfYear < person.DateOfBirth.Value.DayOfYear ? 1 : 0) : null,
                    Gender = person.Gender,
                    CountryName = person.Country?.CountryName
                };
            }
        }
    
}
