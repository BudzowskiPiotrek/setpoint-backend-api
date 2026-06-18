using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewUsersInvitationEstructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER_INVITATIONS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DELETED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EMAIL = table.Column<string>(type: "text", nullable: false),
                    TOKEN = table.Column<Guid>(type: "uuid", nullable: false),
                    SENDER_USER_ID = table.Column<Guid>(type: "uuid", nullable: true),
                    EXPIRES_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    STATUS = table.Column<int>(type: "integer", nullable: false),
                    SENDED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_INVITATIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_USER_INVITATIONS_USERS_SENDER_USER_ID",
                        column: x => x.SENDER_USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATIONS_SENDER_USER_ID",
                table: "USER_INVITATIONS",
                column: "SENDER_USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USER_INVITATIONS");
        }
    }
}
