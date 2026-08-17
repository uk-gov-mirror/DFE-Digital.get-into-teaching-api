using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddReferenceNumberToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceNumber",
                table: "TeachingEvents",
                type: "integer",
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
