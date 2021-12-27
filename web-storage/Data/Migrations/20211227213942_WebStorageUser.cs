using Microsoft.EntityFrameworkCore.Migrations;

namespace web_storage.Data.Migrations
{
    public partial class WebStorageUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedPhoneNumber",
                table: "AspNetUsers");
        }
    }
}
