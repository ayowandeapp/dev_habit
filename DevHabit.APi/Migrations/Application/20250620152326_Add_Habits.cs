using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.APi.Migrations.Application
{
    /// <inheritdoc />
    public partial class Add_Habits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "habits",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false),
                    frequency_type = table.Column<int>(type: "int", nullable: false),
                    frequency_times_per_period = table.Column<int>(type: "int", nullable: false),
                    target_value = table.Column<int>(type: "int", nullable: false),
                    target_unit = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    is_archived = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    milestone_target = table.Column<int>(type: "int", nullable: true),
                    milestone_current = table.Column<int>(type: "int", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    last_completed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_habits", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "habits");
        }
    }
}
