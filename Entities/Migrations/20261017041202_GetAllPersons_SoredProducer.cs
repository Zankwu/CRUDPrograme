using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetAllPersons_SoredProducer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp = @"
                CREATE PROCEDURE [dbo].[GetAllPersons]
                AS
                BEGIN
                    SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters 
                    FROM [dbo].[Persons]
                END
            ";
            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sql = "DROP PROCEDURE IF EXISTS dbo.GetAllPersons;";
            migrationBuilder.Sql(sql);
        }
    }
}
