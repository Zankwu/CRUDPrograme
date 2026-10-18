using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryService
    {
        public Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        public Task<List<CountryResponse>> GetAllCountries();

        public Task<CountryResponse> GetCountryByID(Guid guid);

        public Task<int> UploadCountryByExecl(IFormFile formFile);

    }
}
