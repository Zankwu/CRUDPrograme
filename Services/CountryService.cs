using Entities;
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

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
            _db.SaveChanges();
            return country.ToCountryResponse();

        }



        public List<CountryResponse> GetAllCountries()
        {


            return _db.Countries.Select(item=>item.ToCountryResponse()).ToList();
        }

        public CountryResponse GetCountryByID(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return null;
            }
            Country countryBySelect = _db.Countries.Where(item => item.CountryId == guid).First();
            CountryResponse countryResponseBySelect = countryBySelect.ToCountryResponse();
            return countryResponseBySelect;
        }
    }
}
