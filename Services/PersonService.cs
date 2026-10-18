using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
namespace Services
{
    public class PersonService : IPersonService
    {


        private readonly ApplicationDbContext _db;
        public PersonService(ApplicationDbContext db)
        {
            _db = db;
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

            if (_db.Persons.Where(item => item.PersonID == person.PersonID).Count() > 0)
            {
                throw new ArgumentException();
            }
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid personID)
        {
            if (personID == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            Person? person_delete = await _db.Persons.FirstOrDefaultAsync(item => item.PersonID == personID);
            if (person_delete == null)
            {
                return false;
            }
            _db.Persons.Remove(person_delete);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PersonResponse>> GetAllPerson()
        {


            List<Person> people = await _db.Persons.Include("Country").ToListAsync();

            List<PersonResponse> personResponses = _db.sp_GetAllPersons().Select
                (item => item.ToPersonResponse()).ToList();


            return personResponses;
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {

            // 1. 获取所有人员作为基准数据源
            List<PersonResponse> allPersons = await GetAllPerson();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(item => !string.IsNullOrEmpty(item.PersonName) ?
                    (item.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) :
                    true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(item => !string.IsNullOrEmpty(item.Email) ?
                    (item.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)) :
                    true).ToList();
                    break;

                case nameof(PersonResponse.CountryName):
                    matchingPersons = allPersons.Where(item => !string.IsNullOrEmpty(searchString) ?
                    (item.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) :
                    true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(item => !string.IsNullOrEmpty(searchString) ?
                    item.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) :
                    true).ToList();
                    break;
            }

            return matchingPersons;
        }

        public async Task<PersonResponse> GetPersonByID(Guid personID)
        {
            //throw new NotImplementedException();
            if (personID == Guid.Empty)
            {
                return null;
            }
            Person? personByID = await _db.Persons.FirstOrDefaultAsync(item => item.PersonID == personID);
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
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);
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

            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(item => item.PersonID == personUpdateRequest.PersonId);

            if (matchingPerson == null)
            {
                throw new ArgumentException();
            }
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString(); // 枚举转字符串
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            await _db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }

        public async Task<MemoryStream> GetPersonExcel()
        {
            List<PersonResponse> personResponses_Get = await GetAllPerson();
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
                worksheet.Cells["B1"].Value = nameof(PersonResponse.CountryName);
                worksheet.Cells["C1"].Value = nameof(PersonResponse.Gender);
                int row = 2;

                foreach (var person in personResponses_Get)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.CountryName;
                    worksheet.Cells[row, 3].Value = person.Gender;
                    row++;
                }
                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
