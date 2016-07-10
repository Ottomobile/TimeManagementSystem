using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeManagementSystem.Migrations
{
    public partial class V20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "TimeRecord",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TimeRecord",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "PayPeriod",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "TimeRecord",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TimeRecord",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "PayPeriod",
                nullable: true);
        }
    }
}
