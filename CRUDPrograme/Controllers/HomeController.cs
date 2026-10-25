using CRUDPrograme.Filters;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Diagnostics;

namespace CRUDPrograme.Controllers
{
    //[TypeFilter(typeof(ResponseHeaderAsyncActionFilter),
    //    Arguments = new object[] { "X-Custom-Controller", "My-Custom-Controller" ,2},Order =2)
    //    ]
    public class HomeController : Controller
    {
        private readonly IPersonService _personService;
        private ILogger<HomeController> _logger;

        public HomeController(IPersonService personService, ILogger<HomeController> logger  )
        {
            _personService = personService;
            _logger = logger;
        }
        //[Route("/")]
        //public async Task<IActionResult> Index()
        //{
        //    MemoryStream memoryStream = await _personService.GetPersonCsvCustomer();
        //    return File(memoryStream, "text/csv", "persons.csv");
        //}

        [Route("/csv")]
        public async Task<IActionResult> csv()
        {
            MemoryStream memoryStream = await _personService.GetPersonExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "persons.xlsx");
        }

        [Route("/")]
        [TypeFilter(typeof(PersonActionFilter))]
        [TypeFilter(typeof(ResponseHeaderAsyncActionFilter),Arguments = 
            new object[] { "X-Custom-Key", "My-Custom-Value" ,1},Order =1)]
        public async Task<IActionResult> Index(string Auth)
        {
            
            var people = await _personService.GetAllPerson();
            string peopleAllName = "";
            foreach (var item in people)
            {
                peopleAllName += item.PersonName;
            }
            return Content(peopleAllName);
        }

        [Route("/country")]
        [TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> GetCountryName()
        {

            var people = await _personService.GetAllPerson();
            string peopleAllName = "";
            foreach (var item in people)
            {
                peopleAllName += item.CountryName;
            }
            return Content(peopleAllName);
        }
    }
}
