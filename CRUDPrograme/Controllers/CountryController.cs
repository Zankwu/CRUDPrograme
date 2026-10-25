using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDPrograme.Controllers
{
    [Route("[Controller]")]
    public class CountryController : Controller
    {

        private readonly ICountryService _countryService;
        private ILogger<CountryController> _logger;
        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }
        [HttpPost]
        [Route("/uploadFile")]
        public async Task<IActionResult> Index([FromForm(Name = "formFile")] IFormFile formFile)
        {
            int sum = await _countryService.UploadCountryByExecl(formFile);
            if (sum == -1)
            {
                return Content($"files: [{string.Join(",", Request.Form.Files.Select(f => f.Name))}]");

            }
            return Content($"{sum}");
        }
    }
}
