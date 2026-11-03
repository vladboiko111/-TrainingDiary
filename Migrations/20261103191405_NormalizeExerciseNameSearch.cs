
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDiary.Migrations
{
    public partial class NormalizeExerciseNameSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Нормалізуємо назви старих тренувань.
            // LOWER() обробляє латиницю ASCII.
            string expression = "LOWER(TRIM(\"ExerciseName\"))";

            // Додаткова обробка кирилиці.
            const string uppercase =
                "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ" +
                "ЁЫЭЪ";

            foreach (char upper in uppercase)
            {
                char lower = char.ToLowerInvariant(upper);

                expression =
                    $"REPLACE({expression}, '{upper}', '{lower}')";
            }

            // Оновлюємо тільки службове поле пошуку.
            migrationBuilder.Sql(
                $"UPDATE \"Workouts\" " +
                $"SET \"ExerciseNameSearch\" = {expression};"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Дані не повертаємо до попереднього регістру,
            // оскільки це лише службове пошукове поле.
        }
    }
}
