using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EXERCISE",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "text", nullable: true),
                    IMAGE_URL = table.Column<string>(type: "text", nullable: true),
                    EQUIPMENT_TYPE = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXERCISE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MUSCLE_GROUP",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUSCLE_GROUP", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    EMAIL = table.Column<string>(type: "text", nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "text", nullable: false),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    SEX = table.Column<int>(type: "integer", nullable: true),
                    BIRTH_DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HEIGHT = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EXERCISE_MUSCLE_GROUP",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    EXERCISE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    MUSCLE_GROUP_ID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXERCISE_MUSCLE_GROUP", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EXERCISE_MUSCLE_GROUP_EXERCISE_EXERCISE_ID",
                        column: x => x.EXERCISE_ID,
                        principalTable: "EXERCISE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXERCISE_MUSCLE_GROUP_MUSCLE_GROUP_MUSCLE_GROUP_ID",
                        column: x => x.MUSCLE_GROUP_ID,
                        principalTable: "MUSCLE_GROUP",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BODY_MEASUREMENTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    ID_USER = table.Column<Guid>(type: "uuid", nullable: false),
                    DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WEIGHT = table.Column<double>(type: "double precision", nullable: false),
                    MUSCLE_MASS = table.Column<double>(type: "double precision", nullable: true),
                    FAT_MASS = table.Column<double>(type: "double precision", nullable: true),
                    BODY_WATER = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BODY_MEASUREMENTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BODY_MEASUREMENTS_USERS_ID_USER",
                        column: x => x.ID_USER,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOGS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    TYPE = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOGS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ROUTINES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROUTINES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ROUTINES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WORKOUT_SESSIONS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DURATION = table.Column<int>(type: "integer", nullable: false),
                    NOTES = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKOUT_SESSIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WORKOUT_SESSIONS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ROUTINE_Exercises",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    ROUTINE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    EXERCISE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    SORT_ORDER = table.Column<int>(type: "integer", nullable: false),
                    SETS = table.Column<int>(type: "integer", nullable: false),
                    REPS = table.Column<int>(type: "integer", nullable: false),
                    TARGET_WEIGHT = table.Column<double>(type: "double precision", nullable: true),
                    REST_SECOND = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROUTINE_Exercises", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ROUTINE_Exercises_EXERCISE_EXERCISE_ID",
                        column: x => x.EXERCISE_ID,
                        principalTable: "EXERCISE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROUTINE_Exercises_ROUTINES_ROUTINE_ID",
                        column: x => x.ROUTINE_ID,
                        principalTable: "ROUTINES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WORKOUT_EXERCISES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    SESSION_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    EXERCISE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    SORT_ORDER = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKOUT_EXERCISES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WORKOUT_EXERCISES_EXERCISE_EXERCISE_ID",
                        column: x => x.EXERCISE_ID,
                        principalTable: "EXERCISE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WORKOUT_EXERCISES_WORKOUT_SESSIONS_SESSION_ID",
                        column: x => x.SESSION_ID,
                        principalTable: "WORKOUT_SESSIONS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXERCISE_SETS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SYNC_STATUS = table.Column<int>(type: "integer", nullable: true),
                    WORKOUT_EXERCISE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    SET_NUMBER = table.Column<int>(type: "integer", nullable: false),
                    REPS = table.Column<int>(type: "integer", nullable: false),
                    WEIGHT = table.Column<double>(type: "double precision", nullable: true),
                    RPE = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXERCISE_SETS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EXERCISE_SETS_WORKOUT_EXERCISES_WORKOUT_EXERCISE_ID",
                        column: x => x.WORKOUT_EXERCISE_ID,
                        principalTable: "WORKOUT_EXERCISES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BODY_MEASUREMENTS_ID_USER",
                table: "BODY_MEASUREMENTS",
                column: "ID_USER");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_MUSCLE_GROUP_EXERCISE_ID",
                table: "EXERCISE_MUSCLE_GROUP",
                column: "EXERCISE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_MUSCLE_GROUP_MUSCLE_GROUP_ID",
                table: "EXERCISE_MUSCLE_GROUP",
                column: "MUSCLE_GROUP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_SETS_WORKOUT_EXERCISE_ID",
                table: "EXERCISE_SETS",
                column: "WORKOUT_EXERCISE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LOGS_USER_ID",
                table: "LOGS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_Exercises_EXERCISE_ID",
                table: "ROUTINE_Exercises",
                column: "EXERCISE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_Exercises_ROUTINE_ID",
                table: "ROUTINE_Exercises",
                column: "ROUTINE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINES_USER_ID",
                table: "ROUTINES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_EXERCISES_EXERCISE_ID",
                table: "WORKOUT_EXERCISES",
                column: "EXERCISE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_EXERCISES_SESSION_ID",
                table: "WORKOUT_EXERCISES",
                column: "SESSION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_SESSIONS_USER_ID",
                table: "WORKOUT_SESSIONS",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BODY_MEASUREMENTS");

            migrationBuilder.DropTable(
                name: "EXERCISE_MUSCLE_GROUP");

            migrationBuilder.DropTable(
                name: "EXERCISE_SETS");

            migrationBuilder.DropTable(
                name: "LOGS");

            migrationBuilder.DropTable(
                name: "ROUTINE_Exercises");

            migrationBuilder.DropTable(
                name: "MUSCLE_GROUP");

            migrationBuilder.DropTable(
                name: "WORKOUT_EXERCISES");

            migrationBuilder.DropTable(
                name: "ROUTINES");

            migrationBuilder.DropTable(
                name: "EXERCISE");

            migrationBuilder.DropTable(
                name: "WORKOUT_SESSIONS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
