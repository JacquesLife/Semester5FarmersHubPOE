using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Programming3A.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhoneNumberError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Farmers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Farmers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
