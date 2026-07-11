using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Upload_Entity_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SymmetricCipherText",
                table: "Uploads",
                newName: "WrappedKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WrappedKey",
                table: "Uploads",
                newName: "SymmetricCipherText");
        }
    }
}
