using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobertTest.Migrations
{
    /// <inheritdoc />
    public partial class addcustomersimage1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "customers",
                newName: "image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "image",
                table: "customers",
                newName: "Image");
        }
    }
}
