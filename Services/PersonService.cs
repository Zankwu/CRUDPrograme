using Castle.Core.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using RepositoryImpl;
using Serilog;
using Serilog.Extensions.Hosting;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SerilogTimings;
namespace Services
{
    public class PersonService : IPersonService
    {


        private readonly IPersonRespository personRespository;

        private readonly IDiagnosticContext _diagnosticContext;
        private ILogger<PersonService> _logger;

        public PersonService(IPersonRespository pr, ILogger<PersonService> logger, IDiagnosticContext dc)
        {
            personRespository = pr;
            _logger = logger;
            _diagnosticContext = dc;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            List<Person> people = await personRespository.GetAllPersons();
            if (people.Where(item => item.PersonID == person.PersonID).Count() > 0)
            {
                throw new ArgumentException();
            }
            await personRespository.AddPerson(person);

            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid personID)
        {
            if (personID == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            Person? person = await personRespository.GetPersonByPersonID(personID);

            if (person == null)
            {
                return false;
            }

            return await personRespository.DeletePersonByPersonID(person.PersonID); ;
        }

        public async Task<List<PersonResponse>> GetAllPerson()
        {
            List<Person> people = [];
            _logger.LogInformation("[PersonService]method in GetAllPerson");
            using (Operation.Time("Time for GetAllPersons from Database"))
            {
                people = await personRespository.GetAllPersons();
            }

            _diagnosticContext.Set("persons", people);

            return people.Select(item => item.ToPersonResponse()).ToList();



        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
            await personRespository.GetFilteredPersons(temp =>
                temp.PersonName!.Contains(searchString!)),

                nameof(PersonResponse.Email) =>
                    await personRespository.GetFilteredPersons(temp =>
                        temp.Email!.Contains(searchString!)),

                nameof(PersonResponse.DateOfBirth) =>
                    await personRespository.GetFilteredPersons(temp =>
                        temp.DateOfBirth!.Value.ToString("dd MMMM yyyy").Contains(searchString!)),

                nameof(PersonResponse.Gender) =>
                    await personRespository.GetFilteredPersons(temp =>
                        temp.Gender!.Contains(searchString!)),

                nameof(PersonResponse.CountryID) =>
                    await personRespository.GetFilteredPersons(temp =>
                        temp.Country!.CountryName!.Contains(searchString!)),



                //_ => await personRespository.GetAllPersons()
            };
            





            return persons.Select(p => p.ToPersonResponse()).ToList(); 
        }

        public async Task<PersonResponse> GetPersonByID(Guid personID)
        {
            //throw new NotImplementedException();
            if (personID == Guid.Empty)
            {
                return null;
            }
            //List<Person> people = await personRespository.GetAllPersons();

            //Person? personByID = people.FirstOrDefault(item => item.PersonID == personID);
            Person? personByID = await personRespository.GetPersonByPersonID(personID);
            if (personByID==null)
            {
                throw new ArgumentNullException();
            }
            return personByID.ToPersonResponse();
        }
        /// <summary>
        /// 无法控制列的CSV导出方法
        /// </summary>
        /// <returns></returns>
        public async Task<MemoryStream> GetPersonCSVFile()
        {
            List<PersonResponse> pesonsResponse = await GetAllPerson();
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture, leaveOpen: true);

            await csvWriter.WriteRecordsAsync(pesonsResponse);
            csvWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonCsvCustomer()
        {
            List<PersonResponse> people_get = await GetAllPerson();
            CsvConfiguration csvConfiguration =
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    LeaveOpen = true
                };
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);
            csvWriter.WriteField(nameof(PersonResponse.PersonID));
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.CountryName));
            csvWriter.NextRecord();

            foreach (var person in people_get)
            {
                csvWriter.WriteField(person.PersonID);
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.CountryName);
                csvWriter.NextRecord();
            }
            csvWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;

        }

        public async Task<List<PersonResponse>> GetPersonsByOrder(List<PersonResponse> people, string? orderBy, OrderOptions order)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return people;
            }

            List<PersonResponse> personResponsesFromSort = new List<PersonResponse>();
            switch (orderBy)
            {
                case nameof(PersonResponse.PersonName):
                    personResponsesFromSort = order == OrderOptions.ASEC ? people.OrderBy(temp => temp.PersonName).ToList() :
                          people.OrderByDescending(temp => temp.PersonName).ToList();
                    ; break;
            }
            return personResponsesFromSort;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException();
            }
            ValidationHelper.ModelValidation(personUpdateRequest);
            // 先确认这个人存在（关键补的一步）
            Person? personFromRepo = await personRespository.GetPersonByPersonID(personUpdateRequest.PersonId);
            if (personFromRepo == null)
            {
                throw new ArgumentException("给定的 PersonId 不存在", nameof(personUpdateRequest));
            }

            Person person = await personRespository.UpdatePerson(personUpdateRequest.ToPerson());
            return person.ToPersonResponse();
        }

        public async Task<MemoryStream> GetPersonExcel()
        {
            List<PersonResponse> people = await GetAllPerson();
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("pesons");
                worksheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
                worksheet.Cells["B1"].Value = nameof(PersonResponse.Email);
                worksheet.Cells["C1"].Value = nameof(PersonResponse.CountryName);
                int row = 2;
                foreach (var person in people)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    worksheet.Cells[row, 3].Value = person.CountryName;
                    row++;
                }
                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
