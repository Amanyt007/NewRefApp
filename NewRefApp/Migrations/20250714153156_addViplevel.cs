using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class addViplevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VipLevel",
                table: "InvestmentPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VipLevel",
                table: "InvestmentPlan");
        }
    }
}
