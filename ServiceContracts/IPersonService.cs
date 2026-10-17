using ServiceContracts.DTO;
using ServiceContracts.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonService
    {

        public Task<PersonResponse> AddPerson(PersonAddRequest personAddRequest);

        public Task<List<PersonResponse>> GetAllPerson();

        /// <summary> /// 根据 PersonID 查询并返回对应的人员详细信息 DTO /// </summary> /// <param />待检索的人员唯一标识符 (GUID) ///
        public Task<PersonResponse> GetPersonByID(Guid personID);

        public Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        public Task<List<PersonResponse>> GetPersonsByOrder(List<PersonResponse> people,string? searchString,OrderOptions order);

        public Task<PersonResponse> UpdatePerson (PersonUpdateRequest personUpdateRequest);

        public Task<bool> DeletePerson(Guid personID);

        public Task<MemoryStream> GetPersonCSVFile();
    }
}
