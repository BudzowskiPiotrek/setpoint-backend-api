using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetPoint.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxSetsToMuscleGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MAX_SETS",
                table: "MUSCLE_GROUP",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<int>(
                name: "MIN_SETS",
                table: "MUSCLE_GROUP",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MAX_SETS",
                table: "MUSCLE_GROUP");

            migrationBuilder.DropColumn(
                name: "MIN_SETS",
                table: "MUSCLE_GROUP");
        }
    }
}
