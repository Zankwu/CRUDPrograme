using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using RepositoryImpl;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUDTests
{
    public class CountryTests
    {
        private readonly CountryService _countryService;
        private readonly ApplicationDbContext applicationDbContext;

        private readonly Mock<ICountryRepository> countryRepositroyMock;
        private readonly ICountryRepository countryRepositroy;

        private readonly IFixture fixture;
        public CountryTests()
        {
            countryRepositroyMock = new Mock<ICountryRepository>();
            countryRepositroy = countryRepositroyMock.Object;
            fixture = new Fixture();


            _countryService = new CountryService(countryRepositroy);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest country = null;


            Func<Task>  act= () => _countryService.AddCountry(country);
            await act.Should().ThrowAsync<ArgumentNullException>();
            
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest country = 
            fixture.Build<CountryAddRequest>()
                .With(item=>item.CountryName,  null as string)
                .Create();
            Func<Task> act = () => _countryService.AddCountry(country);
            await act.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>(() => _countryService.AddCountry(country));
        }

        /// <summary>
        /// 第一次传入成功，第二次传入重复名称则失败
        /// </summary>
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest countryAddRequest = fixture.Build<CountryAddRequest>().With(itme=>itme.CountryName,"mali").Create();

            Country country = countryAddRequest.ToCountry();

            countryRepositroyMock.Setup(item => item.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            Func<Task> act = async () =>
            {
                await _countryService.AddCountry(countryAddRequest);
                await _countryService.AddCountry(countryAddRequest);
            };


            await act.Should().ThrowAsync<ArgumentException>();


        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            CountryAddRequest country_add_front = fixture.Build<CountryAddRequest>()
                .With(item => item.CountryName, "japan")
                .Create();
            Country country = country_add_front.ToCountry();
            CountryResponse countryResponse_expected = country.ToCountryResponse();
            List<Country> countries = [];
            countryRepositroyMock.Setup(item => item.AddCountry(It.IsAny<Country>())).
                ReturnsAsync((Country c) => {countries.Add(c); return c; });
            countryRepositroyMock.Setup(item => item.GetAllCountires()).ReturnsAsync(countries.ToList());

            CountryResponse countryResponse_acturl = await _countryService.AddCountry(country_add_front);
            countryResponse_acturl.CountryId.Should().NotBe(Guid.Empty);
            countryResponse_expected.CountryId = countryResponse_acturl.CountryId;
            countryResponse_acturl.Should().BeEquivalentTo(countryResponse_expected);

            //Assert.True(countryResponse_add_back.CountryId != Guid.Empty);
            //Assert.Equal(expectedCountryResponse, countryResponse_add_back);


        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_Empty()
        {
            List<Country> countries = new List<Country>();
            countryRepositroyMock.Setup(m => m.GetAllCountires()).ReturnsAsync(countries);

            //act
            List<CountryResponse> cresponse = await _countryService.GetAllCountries();

            //Assert
            Assert.Empty(cresponse);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountires()
        {
            List<CountryResponse> addCountries = new List<CountryResponse>();

            //act
            List<CountryAddRequest> crequest = [
                new CountryAddRequest() { CountryName = "usa"},
                new CountryAddRequest() { CountryName = "uk"},
                new CountryAddRequest() { CountryName = "japan"}
                ];
            List<Country> store = [];

            countryRepositroyMock.Setup(item => item.AddCountry(It.IsAny<Country>())).ReturnsAsync(((Country c) => { store.Add(c); return c; }) );
            countryRepositroyMock.Setup(item => item.GetAllCountires()).ReturnsAsync(()=>store.ToList());


            foreach (var item in crequest)
            {
                addCountries.Add(await _countryService.AddCountry(item));
            }

            List<CountryResponse> acturlCountryResponses = await _countryService.GetAllCountries();

            //Assert
            foreach (var item in addCountries)
            {
                Assert.Contains(item, acturlCountryResponses);
            }
        }

        #endregion

        #region GetCountryByID
        [Fact]
        public async Task GetCountry_CountryIDisNull()
        {
            Guid countryID = Guid.Empty;

            Assert.Null(await _countryService.GetCountryByID(countryID));

        }

        [Fact]
        public async Task GetCountryByID()
        {
            CountryAddRequest request = fixture.Build<CountryAddRequest>()
                .With(item => item.CountryName, "china")
                .Create();

            List<Country> store = [];
            countryRepositroyMock.Setup(item => item.AddCountry(It.IsAny<Country>())).ReturnsAsync((Country c) => { store.Add(c); return c; });
            countryRepositroyMock.Setup(item => item.GetAllCountires()).ReturnsAsync(() => store.ToList());

            CountryResponse countryResponseByAdd = await _countryService.AddCountry(request);
            CountryResponse countryResponseByGet = await _countryService.GetCountryByID(countryResponseByAdd.CountryId);

            Assert.Equal(countryResponseByAdd, countryResponseByGet);
        }
        #endregion


    }
}
