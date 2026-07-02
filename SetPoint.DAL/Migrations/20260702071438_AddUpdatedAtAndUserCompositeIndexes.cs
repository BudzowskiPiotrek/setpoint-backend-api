using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtAndUserCompositeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WORKOUT_SESSIONS_USER_ID",
                table: "WORKOUT_SESSIONS");

            migrationBuilder.DropIndex(
                name: "IX_USERS_RELATIONS_FRIEND_ID",
                table: "USERS_RELATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USERS_RELATIONS_USER_ID",
                table: "USERS_RELATIONS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINES_USER_ID",
                table: "ROUTINES");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_REQUESTS_RECEIVER_ID",
                table: "ROUTINE_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_REQUESTS_SENDER_ID",
                table: "ROUTINE_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_BODY_MEASUREMENTS_ID_USER",
                table: "BODY_MEASUREMENTS");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_SESSIONS_UPDATED_AT",
                table: "WORKOUT_SESSIONS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_SESSIONS_USER_ID_UPDATED_AT",
                table: "WORKOUT_SESSIONS",
                columns: new[] { "USER_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_EXERCISES_UPDATED_AT",
                table: "WORKOUT_EXERCISES",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_FRIEND_ID_UPDATED_AT",
                table: "USERS_RELATIONS",
                columns: new[] { "FRIEND_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_UPDATED_AT",
                table: "USERS_RELATIONS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_USER_ID_UPDATED_AT",
                table: "USERS_RELATIONS",
                columns: new[] { "USER_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_USERS_UPDATED_AT",
                table: "USERS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATIONS_UPDATED_AT",
                table: "USER_INVITATIONS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINES_UPDATED_AT",
                table: "ROUTINES",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINES_USER_ID_UPDATED_AT",
                table: "ROUTINES",
                columns: new[] { "USER_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_RECEIVER_ID_UPDATED_AT",
                table: "ROUTINE_REQUESTS",
                columns: new[] { "RECEIVER_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_SENDER_ID_UPDATED_AT",
                table: "ROUTINE_REQUESTS",
                columns: new[] { "SENDER_ID", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_UPDATED_AT",
                table: "ROUTINE_REQUESTS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_Exercises_UPDATED_AT",
                table: "ROUTINE_Exercises",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_MUSCLE_GROUP_UPDATED_AT",
                table: "MUSCLE_GROUP",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_LOGS_UPDATED_AT",
                table: "LOGS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_FEED_EVENTS_UPDATED_AT",
                table: "FEED_EVENTS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_SETS_UPDATED_AT",
                table: "EXERCISE_SETS",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_MUSCLE_GROUP_UPDATED_AT",
                table: "EXERCISE_MUSCLE_GROUP",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_EXERCISE_UPDATED_AT",
                table: "EXERCISE",
                column: "UPDATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_BODY_MEASUREMENTS_ID_USER_UPDATED_AT",
                table: "BODY_MEASUREMENTS",
                columns: new[] { "ID_USER", "UPDATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_BODY_MEASUREMENTS_UPDATED_AT",
                table: "BODY_MEASUREMENTS",
                column: "UPDATED_AT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WORKOUT_SESSIONS_UPDATED_AT",
                table: "WORKOUT_SESSIONS");

            migrationBuilder.DropIndex(
                name: "IX_WORKOUT_SESSIONS_USER_ID_UPDATED_AT",
                table: "WORKOUT_SESSIONS");

            migrationBuilder.DropIndex(
                name: "IX_WORKOUT_EXERCISES_UPDATED_AT",
                table: "WORKOUT_EXERCISES");

            migrationBuilder.DropIndex(
                name: "IX_USERS_RELATIONS_FRIEND_ID_UPDATED_AT",
                table: "USERS_RELATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USERS_RELATIONS_UPDATED_AT",
                table: "USERS_RELATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USERS_RELATIONS_USER_ID_UPDATED_AT",
                table: "USERS_RELATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USERS_UPDATED_AT",
                table: "USERS");

            migrationBuilder.DropIndex(
                name: "IX_USER_INVITATIONS_UPDATED_AT",
                table: "USER_INVITATIONS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINES_UPDATED_AT",
                table: "ROUTINES");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINES_USER_ID_UPDATED_AT",
                table: "ROUTINES");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_REQUESTS_RECEIVER_ID_UPDATED_AT",
                table: "ROUTINE_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_REQUESTS_SENDER_ID_UPDATED_AT",
                table: "ROUTINE_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_REQUESTS_UPDATED_AT",
                table: "ROUTINE_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_ROUTINE_Exercises_UPDATED_AT",
                table: "ROUTINE_Exercises");

            migrationBuilder.DropIndex(
                name: "IX_MUSCLE_GROUP_UPDATED_AT",
                table: "MUSCLE_GROUP");

            migrationBuilder.DropIndex(
                name: "IX_LOGS_UPDATED_AT",
                table: "LOGS");

            migrationBuilder.DropIndex(
                name: "IX_FEED_EVENTS_UPDATED_AT",
                table: "FEED_EVENTS");

            migrationBuilder.DropIndex(
                name: "IX_EXERCISE_SETS_UPDATED_AT",
                table: "EXERCISE_SETS");

            migrationBuilder.DropIndex(
                name: "IX_EXERCISE_MUSCLE_GROUP_UPDATED_AT",
                table: "EXERCISE_MUSCLE_GROUP");

            migrationBuilder.DropIndex(
                name: "IX_EXERCISE_UPDATED_AT",
                table: "EXERCISE");

            migrationBuilder.DropIndex(
                name: "IX_BODY_MEASUREMENTS_ID_USER_UPDATED_AT",
                table: "BODY_MEASUREMENTS");

            migrationBuilder.DropIndex(
                name: "IX_BODY_MEASUREMENTS_UPDATED_AT",
                table: "BODY_MEASUREMENTS");

            migrationBuilder.CreateIndex(
                name: "IX_WORKOUT_SESSIONS_USER_ID",
                table: "WORKOUT_SESSIONS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_FRIEND_ID",
                table: "USERS_RELATIONS",
                column: "FRIEND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_RELATIONS_USER_ID",
                table: "USERS_RELATIONS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINES_USER_ID",
                table: "ROUTINES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_RECEIVER_ID",
                table: "ROUTINE_REQUESTS",
                column: "RECEIVER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROUTINE_REQUESTS_SENDER_ID",
                table: "ROUTINE_REQUESTS",
                column: "SENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BODY_MEASUREMENTS_ID_USER",
                table: "BODY_MEASUREMENTS",
                column: "ID_USER");
        }
    }
}
