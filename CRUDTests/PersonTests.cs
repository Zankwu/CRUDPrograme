

using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using RepositoryContracts;
using RepositoryImpl;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services;
using System.Linq.Expressions;
using Xunit;
using Xunit.Sdk;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PersonTests
{
    public class PersonTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountryService countryService;
        private readonly IPersonService personService;
        private readonly IPersonRespository personRespository;

        private readonly ICountryRepository countryRespository;

        private IFixture _fixture;

        private readonly Mock<IPersonRespository> personRepositoryMock;

        private readonly Mock<ICountryRepository> countryRepositoryMock;
        private readonly Mock<IDiagnosticContext> diagnosticContextMock;
        private readonly Mock<ILogger<PersonService>> loggerMock;

        private readonly ITestOutputHelper _outputHelper;



        public PersonTests(ITestOutputHelper outputHelper)
        {
            personRepositoryMock = new Mock<IPersonRespository>();
            personRespository = personRepositoryMock.Object;

            countryRepositoryMock = new Mock<ICountryRepository>();

            countryRespository = countryRepositoryMock.Object;
            _outputHelper = outputHelper;

            //// 1.模拟的数据，用列表来声明
            //List<Country> countries = new List<Country>();
            //List<Person> People = new List<Person>();

            ////模拟DB上下文，ApplicationDbContext
            //DbContextMock<ApplicationDbContext> dbContextMock =
            //    new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .Options);

            //创建DBSET
            //dbContextMock.CreateDbSetMock(item=>item.Countries, countries);
            //dbContextMock.CreateDbSetMock(item=>item.Persons, People);

            ////获取dbcontext
            //ApplicationDbContext appdb = dbContextMock.Object;
            diagnosticContextMock = new Mock<IDiagnosticContext>();
            loggerMock = new Mock<ILogger<PersonService>>();
            //传入
            countryService = new CountryService(countryRespository);
            personService = new PersonService(personRespository, loggerMock.Object,diagnosticContextMock.Object);
            _fixture = new Fixture();
        }


        #region AddPerson
        [Fact]
        public async Task AddPerson_PersonIsNull()
        {
            PersonAddRequest addRequest = null;
            Func<Task> act =
                () => personService.AddPerson(addRequest);
            await act.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>(
            //    () => personService.AddPerson(addRequest)
            //    );
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            PersonAddRequest addRequest = _fixture.Build<PersonAddRequest>().With(item => item.PersonName, null as string).Create();
            Func<Task> act =
                () => personService.AddPerson(addRequest);

            await act.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(
            //    () => personService.AddPerson(addRequest)
            //    );
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetail()
        {

            // Arrange: 准备包含姓名、邮箱、地址、国家、生日及枚举性别的合法数据
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().Create();


            Person person = personAddRequest.ToPerson();
            personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());

            PersonResponse response = await personService.AddPerson(personAddRequest);
            //List<PersonResponse> personResponsesFromGet = await personService.GetAllPerson();

            //Assert.True(response.PersonID != Guid.Empty);
            response.PersonID.Should().NotBe(Guid.Empty);

        }
        #endregion

        #region GetPerson
        [Fact]
        public async Task GetPersonByID_IDIsNull()
        {
            Guid personID = Guid.Empty;
            (await personService.GetPersonByID(personID)).Should().BeNull();
            //Assert.Null(await personService.GetPersonByID(personID));

        }
        [Fact]
        public async Task GetPersonByID()
        {

            //CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>().With(item=> item.CountryName,"China").Create();
            //CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);
            PersonAddRequest request = _fixture.Build<PersonAddRequest>().Create();
            List<Person> sort = [];
            personRepositoryMock.Setup(item => item.AddPerson(It.IsAny<Person>())).ReturnsAsync((Person p) => { sort.Add(p); return p; });
            personRepositoryMock.Setup(item => item.GetAllPersons()).ReturnsAsync(() => sort.ToList());

            PersonResponse personResponseFrom_Add = await personService.AddPerson(request);
            PersonResponse personResponseFrom_Get = await personService.GetPersonByID(personResponseFrom_Add.PersonID);
            (personResponseFrom_Get).Should().BeEquivalentTo(personResponseFrom_Add);
            //Assert.Equal(personResponseFromAdd, await personService.GetPersonByID(personResponseFromAdd.PersonID));

        }

        [Fact]
        public async Task GetAllPersons_ListIsNull()
        {
            personRepositoryMock.Setup(item => item.GetAllPersons()).ReturnsAsync(new List<Person>());
            List<PersonResponse> personList = await personService.GetAllPerson();
            //Assert.Empty(personList);
            personList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons_ToBeSuccessful()
        {


            // Arrange: 2. 准备包含 3 个不同人员的添加请求集合
            List<PersonAddRequest> personAddRequests = await createCountryForPerson();
            _outputHelper.WriteLine("Expected Persons:");
            List<PersonResponse> personResponseFromAdd = new List<PersonResponse>();



            List<Person> pl = [];
            personRepositoryMock.Setup(item => item.AddPerson(It.IsAny<Person>())).ReturnsAsync((Person p) => { pl.Add(p); return p; });
            personRepositoryMock.Setup(item => item.GetAllPersons()).ReturnsAsync(() => pl.ToList());
            foreach (var item in personAddRequests)
            {
                personResponseFromAdd.Add(await personService.AddPerson(item));
            }
            List<PersonResponse> personResponsesFromGet = await personService.GetAllPerson();
            //foreach (var item in personResponsesFromGet)
            //{
            //    _outputHelper.WriteLine(item.ToString());
            //}

            personResponsesFromGet.Should().BeEquivalentTo(personResponseFromAdd);
            //foreach (var item in personResponseFromAdd)
            //{
            //    _outputHelper.WriteLine(item.ToString());
            //    Assert.Contains(item, personResponsesFromGet);
            //}

        }
        #endregion

        #region GetFileterPerson
        [Fact]
        public async Task GetFilterPersonsBySearch_EmptySeachText()
        {
            var persons = new List<Person>(){
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith1@example.com")
                .With(item=>item.Country,null as Country)
                .Create(),
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith2@example.com")
                .With(item=>item.Country,null as Country)
                .Create(),
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith3@example.com")
                .With(item=>item.Country,null as Country)
                .Create()
            };
            personRepositoryMock
    .Setup(x => x.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
    .ReturnsAsync(persons);
            List<PersonResponse> temp = [];
            foreach (var item in persons)
            {
                temp.Add(item.ToPersonResponse());
            }

            List<PersonResponse> personResponseListFromGet =
                await personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");

            personResponseListFromGet.Should().BeEquivalentTo(temp);
            //foreach (var item in personResponsesFromAdd)
            //{
            //    Assert.Contains(item, personResponseListFromGet);
            //}
        }

        private async Task<List<PersonAddRequest>> createCountryForPerson()
        {
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            List<Country> sort = [];
            countryRepositoryMock.Setup(item => item.AddCountry(It.IsAny<Country>())).ReturnsAsync((Country p) => { sort.Add(p); return p; });
            countryRepositoryMock.Setup(item => item.GetAllCountires()).ReturnsAsync(() => sort.ToList());
            CountryResponse countryResponse1 = await countryService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await countryService.AddCountry(countryAddRequest2);

            return new List<PersonAddRequest>(){
                new PersonAddRequest()
                {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "Address of Smith",
                CountryID = countryResponse1.CountryId,
                DateOfBirth = DateTime.Parse("1993-01-02"),

                },
                new PersonAddRequest()
                {
                    PersonName = "Mary",
                    Email = "mary@example.com",
                    Gender = GenderOptions.Female,
                    Address = "Address of Mary",
                    CountryID = countryResponse1.CountryId,
                    DateOfBirth = DateTime.Parse("1990-05-15"),

                },
                new PersonAddRequest()
                {
                    PersonName = "Rahman",
                    Email = "rahman@example.com",
                    Gender = GenderOptions.Male,
                    Address = "Address of Rahman",
                    CountryID = countryResponse2.CountryId,
                    DateOfBirth = DateTime.Parse("1996-12-10"),

                }
            };
        }

        [Fact]
        public async Task GetFilterPersonsSearchByName()
        {
            var persons = new List<Person>(){
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith1@example.com")
                .With(item=>item.Country,null as Country)
                .With(item=>item.PersonName,"maa")

                .Create(),
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith2@example.com")
                .With(item=>item.Country,null as Country)
                .With(item=>item.PersonName,"liii")

                .Create(),
                _fixture.Build<Person>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith3@example.com")
                .With(item=>item.Country,null as Country)
                .With(item=>item.PersonName,"liii1")

                .Create()
            };
            personRepositoryMock
    .Setup(x => x.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
    .ReturnsAsync(persons);
            List<PersonResponse> temp = [];
            foreach (var item in persons)
            {
                temp.Add(item.ToPersonResponse());
            }

            List<PersonResponse> personResponseListFromGet =
                await personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "li");

            personResponseListFromGet.Should().BeEquivalentTo(temp);

        }
        #endregion

        #region sortPerson
        [Fact]
        public async Task GetPersonsList_BySorted()
        {
            var persons = new List<PersonResponse>(){
                _fixture.Build<PersonResponse>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith1@example.com")
                .With(item=>item.PersonName,"maa")

                .Create(),
                _fixture.Build<PersonResponse>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith2@example.com")
                .With(item=>item.PersonName,"liii")

                .Create(),
                _fixture.Build<PersonResponse>().With(item=>item.Gender,"Male")
                .With(item=>item.Email,"smith3@example.com")
                .With(item=>item.PersonName,"liii1")

                .Create()
            };


            //新增的对象返回的personResponse 给添加进 Add返回列表


            //根据匹配规则，返回的列表
            List<PersonResponse> personResponseListFromSort =
                await personService.GetPersonsByOrder(persons, nameof(PersonResponse.PersonName), OrderOptions.DESC);
            //然后在判断新增的代码是否成功匹配

            List<PersonResponse> personResponsesFromAdd_v2 =
                 persons.OrderByDescending(temp => temp.PersonName).ToList();

            //for (int i = 0; i < personResponsesFromAdd_v2.Count; i++)
            //{
            //    Assert.Equal(personResponsesFromAdd_v2[i], personResponseListFromSort[i]);
            //}
            personResponsesFromAdd_v2.Should().BeEquivalentTo(personResponseListFromSort);

        }
        #endregion

        [Fact]
        public async Task UpdatePerson_ObjNull()
        {

            PersonUpdateRequest personUpdateRequest = null;


            await Assert.ThrowsAsync<ArgumentNullException>(() => personService.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_PersonId_UnUse()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>().With
                 (item => item.Email, "smith@example.com")
                 .Create();

            Func<Task> act = async () =>
            {
                await personService.UpdatePerson(personUpdateRequest);
            };
            

            personRepositoryMock
    .Setup(item => item.GetPersonByPersonID(It.IsAny<Guid?>()))
    .ReturnsAsync((Person?)null);          // 模拟"库里查不到"
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonName_UnUse()
        {

            PersonResponse personResponse = _fixture.Build<PersonResponse>().With
                (item => item.Email, "smith@example.com")
                .Create();


            //PersonResponse personResponse_add = await personService.AddPerson(personAddRequest);
            personResponse.PersonName = null;
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => personService.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(item=>item.Country,null as Country)
                .Create();

            PersonUpdateRequest personUpdateRequest = person.ToPersonResponse().ToPersonUpdateRequest();

            personRepositoryMock.Setup(item => item.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            personRepositoryMock.Setup(item => item.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse personResponse_V2 = await personService.UpdatePerson(personUpdateRequest);
            PersonResponse person_V3 = await personService.GetPersonByID(personUpdateRequest.PersonId);
            Assert.Equal(personResponse_V2, person_V3);
        }

        #region delete
        [Fact]
        public async Task DeletePerson_PersonIDIsNull()
        {
            Guid personId = Guid.Empty;

            await Assert.ThrowsAsync<ArgumentNullException>(() => personService.DeletePerson(personId));
        }

        [Fact]
        public async Task DeletePerson_Succese()
        {

            Person person = _fixture.Build<Person>()
                .With(item => item.Email, "mary@example.com")
                .With(item => item.Country, null as Country)
                .Create();
            personRepositoryMock.Setup(item => item.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            personRepositoryMock.Setup(item => item.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            bool boolResult = await personService.DeletePerson(person.PersonID);
            boolResult.Should().BeTrue();


        }
        #endregion
    }
}
