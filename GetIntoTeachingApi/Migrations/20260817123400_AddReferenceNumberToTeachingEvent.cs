using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddReferenceNumberToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NB: ReferenceNumber is a string-prefixed number in the CRM, not an integer
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "TeachingEvents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "TeachingEvents");
        }
    }
}
