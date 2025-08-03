using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class addImagetoPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "InvestmentPlan",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "InvestmentPlan");
        }
    }
}
