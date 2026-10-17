using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.DTO
{
    public class CountryResponse
    {

        public Guid CountryId { get; set; }

        public string CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            CountryResponse temp = (CountryResponse)obj;
            return CountryId == temp.CountryId && CountryName == temp.CountryName;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(CountryId, CountryName);
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName
            };
        }
    }

    
}
