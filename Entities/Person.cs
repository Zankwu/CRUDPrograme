using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// 人员数据源实体类
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        // 外键 Guid，在 SQL Server 中对应 uniqueidentifier 类型
        public Guid? CountryID { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        // bool 类型在 SQL Server 内部会自动映射为 bit 类型
        public bool ReceiveNewsLetters { get; set; }
        public int? Age { get; set; }

    }


}
