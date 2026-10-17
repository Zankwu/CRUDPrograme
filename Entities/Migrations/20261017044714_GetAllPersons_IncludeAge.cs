using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetAllPersons_IncludeAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FromSqlRaw 会把返回结果映射到完整实体，实体中所有映射列都必须出现在结果集中，
            // 因此存储过程需要一并返回 Age 列，否则抛 "The required column 'Age' was not present in the results of a 'FromSql' operation."
            string sp = @"
                CREATE OR ALTER PROCEDURE [dbo].[GetAllPersons]
                AS
                BEGIN
                    SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters, Age
                    FROM [dbo].[Persons]
                END
            ";
            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp = @"
                CREATE OR ALTER PROCEDURE [dbo].[GetAllPersons]
                AS
                BEGIN
                    SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters
                    FROM [dbo].[Persons]
                END
            ";
            migrationBuilder.Sql(sp);
        }
    }
}
