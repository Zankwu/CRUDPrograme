using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel;

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

        public async Task<int> UploadCountryByExecl(IFormFile formFile)
        {
            int sum = 0;
            if (formFile == null || formFile.Length == 0)
                return -1;

            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;
                for (int row = 1; row <= rows; row++)
                {
                    string? countryName = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(countryName) && _db.Countries.Where(item=>item.CountryName == countryName).Count()==0)
                    {
                        CountryAddRequest countryAddRequest = new CountryAddRequest()
                        {
                            CountryName = countryName,
                        };
                        _db.Countries.Add(countryAddRequest.ToCountry());
                        sum++;
                    }
                }
            }
            _db.SaveChanges();

            return sum;
        }
    }
}
