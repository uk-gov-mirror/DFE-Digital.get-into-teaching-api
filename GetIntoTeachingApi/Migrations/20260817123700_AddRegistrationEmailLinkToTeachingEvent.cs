using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddRegistrationEmailLinkToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationEmailLink",
                table: "TeachingEvents",
                type: "string",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationEmailLink",
                table: "TeachingEvents");
        }
    }
}
