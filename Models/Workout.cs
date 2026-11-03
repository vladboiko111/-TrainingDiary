
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainingDiary.Models;

public class Workout : IValidatableObject
{
    public int Id { get; set; }

    // Вид спорту
    [Required(ErrorMessage = "Вкажіть вид спорту")]
    [StringLength(60,
        ErrorMessage = "Назва виду спорту занадто довга")]
    public string SportType { get; set; } = string.Empty;

    // Назва вправи
    [Required(ErrorMessage = "Вкажіть назву вправи")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Назва вправи повинна містити від 2 до 100 символів")]
    public string ExerciseName { get; set; } = string.Empty;

    // Категорія тренування
    public WorkoutType Type { get; set; }

    // Тривалість тренування
    [Range(1, 600,
        ErrorMessage = "Тривалість тренування повинна бути від 1 до 600 хвилин")]
    public int DurationMinutes { get; set; }

    // Дистанція тренування
    [Range(typeof(decimal), "0", "1000",
        ErrorMessage = "Дистанція повинна бути від 0 до 1000 км")]
    public decimal? Distance { get; set; }

    // Дата тренування
    public DateTime PerformedOn { get; set; } = DateTime.Today;

    // Зв'язок із довідником категорій спорту
    public int? SportCategoryId { get; set; }

    public SportCategory? SportCategory { get; set; }

    // Зв'язок із довідником вправ
    public int? ExerciseId { get; set; }

    public Exercise? Exercise { get; set; }

    // Власна перевірка взаємозалежних полів
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Type == WorkoutType.Cardio &&
            (!Distance.HasValue || Distance.Value <= 0))
        {
            yield return new ValidationResult(
                "Для кардіотренування вкажіть дистанцію більше 0 км.",
                new[] { nameof(Distance) }
            );
        }
    }
}
