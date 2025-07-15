using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class updateWithdrawModal2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdraw_BankDetails_AdminAccountID",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "UpiNumber",
                table: "Withdraw");

            migrationBuilder.RenameColumn(
                name: "AdminAccountID",
                table: "Withdraw",
                newName: "UpiDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Withdraw_AdminAccountID",
                table: "Withdraw",
                newName: "IX_Withdraw_UpiDetailId");

            migrationBuilder.AddColumn<int>(
                name: "BankDetailId",
                table: "Withdraw",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Withdraw_BankDetailId",
                table: "Withdraw",
                column: "BankDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdraw_BankDetails_BankDetailId",
                table: "Withdraw",
                column: "BankDetailId",
                principalTable: "BankDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdraw_UpiDetails_UpiDetailId",
                table: "Withdraw",
                column: "UpiDetailId",
                principalTable: "UpiDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdraw_BankDetails_BankDetailId",
                table: "Withdraw");

            migrationBuilder.DropForeignKey(
                name: "FK_Withdraw_UpiDetails_UpiDetailId",
                table: "Withdraw");

            migrationBuilder.DropIndex(
                name: "IX_Withdraw_BankDetailId",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "BankDetailId",
                table: "Withdraw");

            migrationBuilder.RenameColumn(
                name: "UpiDetailId",
                table: "Withdraw",
                newName: "AdminAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_Withdraw_UpiDetailId",
                table: "Withdraw",
                newName: "IX_Withdraw_AdminAccountID");

            migrationBuilder.AddColumn<string>(
                name: "UpiNumber",
                table: "Withdraw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdraw_BankDetails_AdminAccountID",
                table: "Withdraw",
                column: "AdminAccountID",
                principalTable: "BankDetails",
                principalColumn: "Id");
        }
    }
}
