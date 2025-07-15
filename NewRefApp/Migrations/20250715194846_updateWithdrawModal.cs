using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class updateWithdrawModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdraw_UpiDetails_AdminUpiId",
                table: "Withdraw");

            migrationBuilder.DropIndex(
                name: "IX_Withdraw_AdminUpiId",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "AdminUpiId",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "BonusPercentage",
                table: "Withdraw");

            migrationBuilder.AddColumn<string>(
                name: "UpiNumber",
                table: "Withdraw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "transactionPassword",
                table: "Withdraw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpiNumber",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "transactionPassword",
                table: "Withdraw");

            migrationBuilder.AddColumn<int>(
                name: "AdminUpiId",
                table: "Withdraw",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "BonusPercentage",
                table: "Withdraw",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_Withdraw_AdminUpiId",
                table: "Withdraw",
                column: "AdminUpiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdraw_UpiDetails_AdminUpiId",
                table: "Withdraw",
                column: "AdminUpiId",
                principalTable: "UpiDetails",
                principalColumn: "Id");
        }
    }
}
