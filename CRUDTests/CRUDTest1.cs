

using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDTests
{
    public class CRUDTest1
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountryService countryService;
        private readonly IPersonService personService;
        private readonly ITestOutputHelper _outputHelper;

        public CRUDTest1(ITestOutputHelper outputHelper)
        {
            

            _outputHelper = outputHelper;

            DbContextOptions<ApplicationDbContext> options =
                            new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CRUDProgrameTest;Integrated Security=True;TrustServerCertificate=True")
                                .Options;

            _db = new ApplicationDbContext(options);

            // ② 保证表结构（含主键、外键）已创建。
            //    若报「No migrations were found」，可临时换成 _db.Database.EnsureCreated();
            _db.Database.Migrate();

            // ③ ★ 清空数据库，让每个测试方法都从"干净的空表"开始。
            //    注意顺序：先删子表 Persons，再删主表 Countries（外键约束）。
            _db.Persons.RemoveRange(_db.Persons);
            _db.Countries.RemoveRange(_db.Countries);
            _db.SaveChanges();
            _db.ChangeTracker.Clear();

            countryService = new CountryService(_db);
            this.personService = new PersonService(_db);

        }
        /// <summary>
        ///  test for country is null
        /// </summary>
        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest country = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => countryService.AddCountry(country));
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest country = new CountryAddRequest() { CountryName = null };

            await Assert.ThrowsAsync<ArgumentNullException>(() => countryService.AddCountry(country));
        }

        /// <summary>
        /// 第一次传入成功，第二次传入重复名称则失败
        /// </summary>
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country2 = new CountryAddRequest() { CountryName = "USA" };


            await Assert.ThrowsAsync<ArgumentException>( async() =>
            {

                 await countryService.AddCountry(country1);
                await countryService.AddCountry(country2);
            }
            );

        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "Japan" };
            CountryResponse countryResponse = await countryService.AddCountry(country1);
            List<CountryResponse> countriesFromGetAllCountires = await countryService.GetAllCountries();

            Assert.True(countryResponse.CountryId != Guid.Empty);
            Assert.Contains(countryResponse, countriesFromGetAllCountires);


        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_Empty()
        {
            //act
            List<CountryResponse> cresponse = await countryService.GetAllCountries();

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
            foreach (var item in crequest)
            {
                addCountries.Add(await countryService.AddCountry(item));
            }

            List<CountryResponse> acturlCountryResponses = await countryService.GetAllCountries();

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

            Assert.Null(await countryService.GetCountryByID(countryID));

        }

        [Fact]
        public async Task GetCountryByID()
        {
            CountryAddRequest request = new CountryAddRequest() { CountryName = "china" };
            CountryResponse countryResponseByAdd = await countryService.AddCountry(request);
            CountryResponse countryResponseByGet = await countryService.GetCountryByID(countryResponseByAdd.CountryId);

            Assert.Equal(countryResponseByAdd, countryResponseByGet);
        }
        #endregion

        #region AddPerson
        [Fact]
        public async Task AddPerson_PersonIsNull()
        {
            PersonAddRequest addRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => personService.AddPerson(addRequest)
                );
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            PersonAddRequest addRequest = new PersonAddRequest()
            {
                PersonName = null
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => personService.AddPerson(addRequest)
                );
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetail()
        {
            CountryAddRequest addRequest = new CountryAddRequest() { CountryName = "KOREA"};
            CountryResponse countryResponse_Add= await countryService.AddCountry(addRequest); 

            // Arrange: 准备包含姓名、邮箱、地址、国家、生日及枚举性别的合法数据
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "张三",
                Email = "zhangsan@example.com",
                Address = "北京市海淀区中关村",
                CountryID = countryResponse_Add.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1995-01-01"),
            };

            PersonResponse response = await personService.AddPerson(personAddRequest);
            List<PersonResponse> personResponsesFromGet = await personService.GetAllPerson();

            Assert.True(response.PersonID != Guid.Empty);

            Assert.Contains(response, personResponsesFromGet);
        }
        #endregion

        #region GetPerson
        [Fact]
        public async Task GetPersonByID_IDIsNull()
        {
            Guid personID = Guid.Empty;
            Assert.Null(await personService.GetPersonByID(personID));

        }
        [Fact]
        public async Task GetPersonByID()
        {

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "China" };
            CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);
            PersonAddRequest request = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                Address = "Beijing, China",
                CountryID = countryResponse.CountryId,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("1998-05-20"),
            };
            PersonResponse personResponseFromAdd = await personService.AddPerson(request);

             Assert.Equal(personResponseFromAdd, await personService.GetPersonByID(personResponseFromAdd.PersonID));

        }

        [Fact]
        public async Task GetAllPersons_ListIsNull()
        {
            List<PersonResponse> personList = await personService.GetAllPerson();
            Assert.Empty(personList);
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons_ToBeSuccessful()
        {


            // Arrange: 2. 准备包含 3 个不同人员的添加请求集合
            List<PersonAddRequest> personAddRequests = await createCountryForPerson();
            _outputHelper.WriteLine("Expected Persons:");
            List<PersonResponse> personResponseFromAdd = new List<PersonResponse>();
            foreach (var item in personAddRequests)
            {

                personResponseFromAdd.Add(await personService.AddPerson(item));
            }

            List<PersonResponse> personResponsesFromGet = await personService.GetAllPerson();
            foreach (var item in personResponsesFromGet)
            {
                _outputHelper.WriteLine(item.ToString());
            }

            foreach (var item in personResponseFromAdd)
            {
                _outputHelper.WriteLine(item.ToString());
                Assert.Contains(item, personResponsesFromGet);
            }

        }
        #endregion

        #region GetFileterPerson
        [Fact]
        public async Task GetFilterPersonsBySearch_EmptySeachText()
        {


            // Arrange: 2. 准备包含 3 个不同人员的添加请求集合
            List<PersonAddRequest> personAddRequests = await createCountryForPerson();

            List<PersonResponse> personResponsesFromAdd = [];
            foreach (var pesonRequest in personAddRequests)
            {
                personResponsesFromAdd.Add(await personService.AddPerson(pesonRequest));
            }

            List<PersonResponse> personResponseListFromGet = await personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");
            foreach (var item in personResponsesFromAdd)
            {
                Assert.Contains(item, personResponseListFromGet);
            }
        }

        private async Task<List<PersonAddRequest>> createCountryForPerson()
        {
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

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
            // Arrange: 2. 准备包含 3 个不同人员的添加请求集合
            List<PersonAddRequest> personAddRequests = await createCountryForPerson();

            //新增的对象返回的personResponse 给添加进 Add返回列表
            List<PersonResponse> personResponsesFromAdd = [];
            foreach (var pesonRequest in personAddRequests)
            {
                personResponsesFromAdd.Add(await personService.AddPerson(pesonRequest));
            }
            //根据匹配规则，返回的列表
            List<PersonResponse> personResponseListFromGet = await personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "ma");
            //然后在判断新增的代码是否成功匹配
            foreach (PersonResponse item in personResponsesFromAdd)
            {
                if (item.PersonName != null)
                {
                    if (item.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(item, personResponseListFromGet);
                    }
                }


            }
        }
        #endregion

        #region sortPerson
        [Fact]
        public async Task GetPersonsList_BySorted()
        {
            // Arrange: 2. 准备包含 3 个不同人员的添加请求集合
            List<PersonAddRequest> personAddRequests = await createCountryForPerson();

            //新增的对象返回的personResponse 给添加进 Add返回列表
            List<PersonResponse> personResponsesFromAdd = [];
            foreach (var pesonRequest in personAddRequests)
            {
                personResponsesFromAdd.Add(await personService.AddPerson(pesonRequest));
            }
            //根据匹配规则，返回的列表
            List<PersonResponse> personResponseListFromSort =
                await personService.GetPersonsByOrder(personResponsesFromAdd, nameof(PersonResponse.PersonName), OrderOptions.DESC);
            //然后在判断新增的代码是否成功匹配

            List<PersonResponse> personResponsesFromAdd_v2 =
                 personResponsesFromAdd.OrderByDescending(temp => temp.PersonName).ToList();

            for (int i = 0; i < personResponsesFromAdd_v2.Count; i++)
            {
                Assert.Equal(personResponsesFromAdd_v2[i], personResponseListFromSort[i]);
            }
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
            CountryResponse country = await countryService.AddCountry(
    new CountryAddRequest() { CountryName = "USA" });

            PersonAddRequest personAddRequest =
            new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "Address of Smith",
                CountryID = country.CountryId,
                DateOfBirth = DateTime.Parse("1993-01-02"),

            };
            PersonResponse personResponse_add = await personService.AddPerson(personAddRequest);
            personResponse_add.PersonID = Guid.NewGuid();
            PersonUpdateRequest personUpdateRequest = personResponse_add.ToPersonUpdateRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => personService.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_PersonName_UnUse()
        {
            CountryResponse country = await countryService.AddCountry(
    new CountryAddRequest() { CountryName = "USA" });
            PersonAddRequest personAddRequest =
            new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "Address of Smith",
                CountryID = country.CountryId,
                DateOfBirth = DateTime.Parse("1993-01-02"),

            };
            PersonResponse personResponse_add = await personService.AddPerson(personAddRequest);
            personResponse_add.PersonName = null;
            PersonUpdateRequest personUpdateRequest = personResponse_add.ToPersonUpdateRequest();

            await Assert.ThrowsAsync<ArgumentException>(() =>  personService.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "china" };
            CountryResponse countryResponse_Add= await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() {
                PersonName = "Mary",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "Address of Mary",
                CountryID = countryResponse_Add.CountryId,
                DateOfBirth = DateTime.Parse("1990-05-15"),
            };

            PersonResponse personResponse =await personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "MaryV2";

            PersonResponse personResponse_V2 = await personService.UpdatePerson(personUpdateRequest);
            PersonResponse person_V3 = await personService.GetPersonByID(personUpdateRequest.PersonId);
            Assert.Equal(personResponse_V2, person_V3);
        }

        #region delete
        [Fact]
        public async Task DeletePerson_PersonIDIsNull()
        {
            Guid personId = Guid.Empty;

            await Assert.ThrowsAsync<ArgumentNullException>(()=>personService.DeletePerson(personId));
        }

        [Fact]
        public async Task DeletePerson_Succese()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "china" };
            CountryResponse countryResponse_Add = await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "Address of Mary",
                CountryID = countryResponse_Add.CountryId,
                DateOfBirth = DateTime.Parse("1990-05-15"),
            };

            PersonResponse personResponse = await personService.AddPerson(personAddRequest);

            Assert.True(await personService.DeletePerson(personResponse.PersonID));
        }
        #endregion
    }
}
