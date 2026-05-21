using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeLogsUserIdNullableAndCleanupBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LOGS_USERS_USER_ID",
                table: "LOGS");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "WORKOUT_SESSIONS");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "WORKOUT_EXERCISES");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "ROUTINES");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "ROUTINE_Exercises");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "MUSCLE_GROUP");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "LOGS");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "EXERCISE_SETS");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "EXERCISE_MUSCLE_GROUP");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "EXERCISE");

            migrationBuilder.DropColumn(
                name: "SYNC_STATUS",
                table: "BODY_MEASUREMENTS");

            migrationBuilder.AlterColumn<Guid>(
                name: "USER_ID",
                table: "LOGS",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_LOGS_USERS_USER_ID",
                table: "LOGS",
                column: "USER_ID",
                principalTable: "USERS",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LOGS_USERS_USER_ID",
                table: "LOGS");

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "WORKOUT_SESSIONS",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "WORKOUT_EXERCISES",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "USERS",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "ROUTINES",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "ROUTINE_Exercises",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "MUSCLE_GROUP",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<Guid>(
                name: "USER_ID",
                table: "LOGS",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "LOGS",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "EXERCISE_SETS",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "EXERCISE_MUSCLE_GROUP",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "EXERCISE",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "SYNC_STATUS",
                table: "BODY_MEASUREMENTS",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddForeignKey(
                name: "FK_LOGS_USERS_USER_ID",
                table: "LOGS",
                column: "USER_ID",
                principalTable: "USERS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
