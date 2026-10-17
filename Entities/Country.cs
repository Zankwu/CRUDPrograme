using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }
        [StringLength(40)]
        public string CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Country))
            {
                return false;
            }
            Country temp = (Country)obj;
            return CountryId == temp.CountryId && CountryName == temp.CountryName;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(CountryId, CountryName);
        }   
    }
}
