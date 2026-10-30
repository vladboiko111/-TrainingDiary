
namespace TrainingDiary.Models;

public class Workout
{
    public int Id { get; set; }

    public string SportType { get; set; } = string.Empty;

    public string ExerciseName { get; set; } = string.Empty;

    public WorkoutType Type { get; set; } = WorkoutType.Strength;

    public int DurationMinutes { get; set; }

    public decimal? Distance { get; set; }

    public DateTime PerformedOn { get; set; } = DateTime.Today;
}
