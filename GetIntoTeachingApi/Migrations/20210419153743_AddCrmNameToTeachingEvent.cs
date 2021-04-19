using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddCrmNameToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrmName",
                table: "TeachingEvents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmName",
                table: "TeachingEvents");
        }
    }
}
