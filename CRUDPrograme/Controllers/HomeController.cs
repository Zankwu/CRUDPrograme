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
        //[Route("/")]
        //public async Task<IActionResult> Index()
        //{
        //    MemoryStream memoryStream = await _personService.GetPersonCsvCustomer();
        //    return File(memoryStream, "text/csv", "persons.csv");
        //}

        [Route("/csv")]
        public async Task<IActionResult> Index()
        {
            MemoryStream memoryStream = await _personService.GetPersonExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "persons.xlsx");
        }
    }
}
