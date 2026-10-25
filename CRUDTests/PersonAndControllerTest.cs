using Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUDTests
{
    public class PersonAndControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonAndControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task index()
        {
            // Act：等价于浏览器地址栏输入 /Persons/Index
            HttpResponseMessage response = await _client.GetAsync("/");

            // Assert：状态码 2xx
            response.Should().BeSuccessful();
        }
    }
}
