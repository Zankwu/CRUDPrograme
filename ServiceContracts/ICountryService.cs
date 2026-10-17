using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryService
    {
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        public List<CountryResponse> GetAllCountries();

        public CountryResponse GetCountryByID(Guid guid);

    }
}
