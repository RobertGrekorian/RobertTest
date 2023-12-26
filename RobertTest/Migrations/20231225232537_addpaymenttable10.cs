using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobertTest.Migrations
{
    /// <inheritdoc />
    public partial class addpaymenttable10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripePaymentId",
                table: "payments",
                newName: "SessionId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "payments",
                newName: "StripePaymentId");
        }
    }
}
