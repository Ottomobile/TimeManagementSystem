using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeManagementSystem.Migrations
{
    public partial class V7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hours",
                table: "PayPeriod");

            migrationBuilder.AddColumn<int>(
                name: "MiscMin",
                table: "PayPeriod",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiscMin",
                table: "PayPeriod");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Hours",
                table: "PayPeriod",
                nullable: true);
        }
    }
}
