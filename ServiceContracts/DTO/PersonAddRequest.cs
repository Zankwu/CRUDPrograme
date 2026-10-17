using Entities;
using ServiceContracts.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="名称不能为空")]
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid CountryID { get; set; }
        public string? Address { get; set; }

        public Person ToPerson()
        {
            return new Person {  PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth ,Gender =  Gender.ToString()};
        }
    }
}
