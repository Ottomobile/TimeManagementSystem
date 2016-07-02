using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TimeManagementSystem.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeRecord",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comments = table.Column<string>(nullable: true),
                    DurationBreak = table.Column<DateTime>(nullable: false),
                    DurationWork = table.Column<DateTime>(nullable: false),
                    RecordDate = table.Column<DateTime>(nullable: false),
                    TimeBreakEnd = table.Column<DateTime>(nullable: false),
                    TimeBreakStart = table.Column<DateTime>(nullable: false),
                    TimeTotal = table.Column<DateTime>(nullable: false),
                    TimeWorkEnd = table.Column<DateTime>(nullable: false),
                    TimeWorkStart = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRecord", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeRecord");
        }
    }
}
