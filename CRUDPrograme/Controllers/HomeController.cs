using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDPrograme.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPersonService _personService;
        public HomeController(IPersonService personService)
        {
            _personService = personService;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            MemoryStream memoryStream = await _personService.GetPersonCSVFile();
            return File(memoryStream, "text/csv", "persons.csv");
        }
    }
}
