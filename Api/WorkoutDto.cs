
namespace TrainingDiary.Api;

public record WorkoutDto(
    int Id,
    string SportType,
    string ExerciseName,
    string Type,
    int DurationMinutes,
    decimal? Distance,
    DateTime PerformedOn
);
