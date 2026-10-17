using ServiceContracts.DTO;
using ServiceContracts.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonService
    {

        public PersonResponse AddPerson(PersonAddRequest personAddRequest);

        public List<PersonResponse> GetAllPerson();

        /// <summary> /// 根据 PersonID 查询并返回对应的人员详细信息 DTO /// </summary> /// <param />待检索的人员唯一标识符 (GUID) ///
        public PersonResponse GetPersonByID(Guid personID);

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

        public List<PersonResponse> GetPersonsByOrder(List<PersonResponse> people,string? searchString,OrderOptions order);

        public PersonResponse UpdatePerson (PersonUpdateRequest personUpdateRequest);

        public bool DeletePerson(Guid personID);

    }
}
