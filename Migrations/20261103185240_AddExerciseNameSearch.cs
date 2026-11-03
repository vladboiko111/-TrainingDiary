
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDiary.Migrations
{
    public partial class AddExerciseNameSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Додаємо поле для пошуку
            migrationBuilder.AddColumn<string>(
                name: "ExerciseNameSearch",
                table: "Workouts",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Заповнюємо пошукове поле для старих тренувань
            migrationBuilder.Sql(@"
                UPDATE Workouts
                SET ExerciseNameSearch = LOWER(ExerciseName);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseNameSearch",
                table: "Workouts");
        }
    }
}
