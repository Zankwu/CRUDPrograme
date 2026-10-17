using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryService : ICountryService
    {

        private readonly ApplicationDbContext _db;

        public CountryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid();

            if (_db.Countries.Count(item => item.CountryName == country.CountryName)>0)
            {
                throw new ArgumentException(nameof(country));
            }
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country.ToCountryResponse();

        }



        public async Task<List<CountryResponse>> GetAllCountries()
        {


            return await _db.Countries.Select(item=>item.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse> GetCountryByID(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return null;
            }
            Country countryBySelect = await _db.Countries.Where(item => item.CountryId == guid).FirstAsync();
            CountryResponse countryResponseBySelect = countryBySelect.ToCountryResponse();
            return countryResponseBySelect;
        }
    }
}
