using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewSocialStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ROUTINE_REQUESTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SENDER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    RECEIVER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ROUTINE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    STATUS = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROUTINE_REQUESTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ROUTINE_REQUESTS_ROUTINES_ROUTINE_ID",
                        column: x => x.ROUTINE_ID,
                        principalTable: "ROUTINES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ROUTINE_REQUESTS_USERS_RECEIVER_ID",
                        column: x => x.RECEIVER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ROUTINE_REQUESTS_USERS_SENDER_ID",
                        column: x => x.SENDER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USERS_RELATIONS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    FRIEND_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    STATUS = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS_RELATIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_USERS_RELATIONS_USERS_FRIEND_ID",
                        column: x => x.FRIEND_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USERS_RELATIONS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_RECEIVER_ID",
                table: "ROUTINE_REQUESTS",
                column: "RECEIVER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_ROUTINE_ID",
                table: "ROUTINE_REQUESTS",
                column: "ROUTINE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_SENDER_ID",
                table: "ROUTINE_REQUESTS",
                column: "SENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_FRIEND_ID",
                table: "USERS_RELATIONS",
                column: "FRIEND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_USER_ID",
                table: "USERS_RELATIONS",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ROUTINE_REQUESTS");

            migrationBuilder.DropTable(
                name: "USERS_RELATIONS");
        }
    }
}
