﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRefApp.Migrations
{
    /// <inheritdoc />
    public partial class addPurchaseQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseQuantity",
                table: "UserInvestment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseQuantity",
                table: "UserInvestment");
        }
    }
}
