using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewFeedEventStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FEED_EVENTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    USER_ID = table.Column<Guid>(type: "uuid", nullable: true),
                    EVENT_TYPE = table.Column<int>(type: "integer", nullable: false),
                    EXERCISE_ID = table.Column<Guid>(type: "uuid", nullable: true),
                    METRIC_VALUE = table.Column<double>(type: "double precision", nullable: true),
                    CUSTOM_TEXT = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FEED_EVENTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FEED_EVENTS_EXERCISE_EXERCISE_ID",
                        column: x => x.EXERCISE_ID,
                        principalTable: "EXERCISE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FEED_EVENTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FEED_EVENTS_EXERCISE_ID",
                table: "FEED_EVENTS",
                column: "EXERCISE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FEED_EVENTS_USER_ID",
                table: "FEED_EVENTS",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FEED_EVENTS");
        }
    }
}
