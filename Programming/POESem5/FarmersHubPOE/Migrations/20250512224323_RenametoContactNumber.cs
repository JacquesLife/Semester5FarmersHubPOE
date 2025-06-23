using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Programming3A.Migrations
{
    /// <inheritdoc />
    public partial class RenametoContactNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Farmers",
                newName: "ContactNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Farmers",
                newName: "PhoneNumber");
        }
    }
}
