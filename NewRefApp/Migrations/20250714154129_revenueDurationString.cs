using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class revenueDurationString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RevenueDurationValue",
                table: "InvestmentPlan",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RevenueDurationValue",
                table: "InvestmentPlan",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
