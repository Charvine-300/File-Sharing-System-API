using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Upload_And_Policy_Entity_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryUrl",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileNonce",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileTag",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemPolicy",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PolicyName",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "CloudinaryUrl",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "FileNonce",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "FileTag",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "IsSystemPolicy",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "PolicyName",
                table: "Policies");
        }
    }
}
