using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryImpl
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _db;

        public CountryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async  Task<Country> AddCountry(Country country)
        {
            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountires()
        {
            
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByID(Guid countryID)
        {
            return await _db.Countries.FirstOrDefaultAsync(item => item.CountryId == countryID);
        }
    }
}
