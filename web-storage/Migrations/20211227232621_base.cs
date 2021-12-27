using Microsoft.EntityFrameworkCore.Migrations;

namespace web_storage.Migrations
{
    public partial class @base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedPhoneNumber",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }
    }
}
