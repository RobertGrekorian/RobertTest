using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobertTest.Migrations
{
    /// <inheritdoc />
    public partial class addpaymenttable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_customers_PaymentId",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_PaymentId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "customers");

            migrationBuilder.CreateIndex(
                name: "IX_payments_customerId",
                table: "payments",
                column: "customerId");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_customers_customerId",
                table: "payments",
                column: "customerId",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_customers_customerId",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_customerId",
                table: "payments");

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_payments_PaymentId",
                table: "payments",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_customers_PaymentId",
                table: "payments",
                column: "PaymentId",
                principalTable: "customers",
                principalColumn: "id");
        }
    }
}
