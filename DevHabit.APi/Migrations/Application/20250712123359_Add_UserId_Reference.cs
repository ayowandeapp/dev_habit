using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.APi.Migrations.Application
{
    /// <inheritdoc />
    public partial class Add_UserId_Reference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM habit_tags;
                DELETE FROM habits;
                DELETE FROM tags;
                """
            );
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "tags",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "habits",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name_user_id",
                table: "tags",
                columns: new[] { "name", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_user_id",
                table: "tags",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_habits_user_id",
                table: "habits",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_habits_users_user_id",
                table: "habits",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tags_users_user_id",
                table: "tags",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_habits_users_user_id",
                table: "habits");

            migrationBuilder.DropForeignKey(
                name: "fk_tags_users_user_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_name_user_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_user_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_habits_user_id",
                table: "habits");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "habits");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);
        }
    }
}
