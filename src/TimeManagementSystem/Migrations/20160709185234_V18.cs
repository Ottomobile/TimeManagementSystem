using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeManagementSystem.Migrations
{
    public partial class V18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ManagingUser",
                table: "SubscribeToUser",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUser",
                table: "SubscribeToUser",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ManagingUser",
                table: "SubscribeToUser",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUser",
                table: "SubscribeToUser",
                maxLength: 450,
                nullable: true);
        }
    }
}
