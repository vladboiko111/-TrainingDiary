using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDiary.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueWorkoutIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ExerciseName_PerformedOn",
                table: "Workouts",
                columns: new[] { "ExerciseName", "PerformedOn" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_ExerciseName_PerformedOn",
                table: "Workouts");
        }
    }
}
