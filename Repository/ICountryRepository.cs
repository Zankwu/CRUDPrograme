using Entities;

namespace Repository
{
    public interface ICountryRepository
    {

        public Task<Country> AddCountry(Country country);

        public Task<List<Country>> GetAllCountires();

        public Task<Country?> GetCountryByID(Guid countryID);


    }
}
