using CRUDPrograme.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUDTests
{
    public class ControllerTests
    {

        private readonly IPersonService _personService;
        private readonly ICountryService _countryService;
        private readonly Mock<ILogger<HomeController>> loggerMock;

        private readonly HomeController _homeController;
        public ControllerTests()
        {
            var personServiceMock = new Mock<IPersonService>();
            var countryServiceMock = new Mock<ICountryService>();

            _personService = personServiceMock.Object;
            _countryService = countryServiceMock.Object;

            _homeController = new HomeController(_personService, loggerMock.Object);
        }
        [Fact]
        public async Task IndexTest()
        {
           IActionResult result = await _homeController.Index();
            result.Should().BeOfType<ContentResult>();
            
        }
    }
}
