
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrainingDiary.Data;
using TrainingDiary.Models;
using TrainingDiary.Services;
using Xunit;

namespace TrainingDiary.Tests;

public class SummaryCalculatorTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TrainingDiaryContext> _options;

    public SummaryCalculatorTests()
    {
        // Окрема тестова база SQLite в оперативній пам'яті.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<TrainingDiaryContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new TrainingDiaryContext(_options);
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    // Допоміжний метод створення тренування.
    private static Workout CreateWorkout(
        string sportType,
        string exerciseName,
        int duration,
        DateTime date)
    {
        return new Workout
        {
            SportType = sportType,
            ExerciseName = exerciseName,
            ExerciseNameSearch = exerciseName.ToLowerInvariant(),
            Type = WorkoutType.Strength,
            DurationMinutes = duration,
            PerformedOn = date
        };
    }

    // ТЕСТ 1: порожня база.
    [Fact]
    public async Task BySport_EmptyDatabase_ReturnsEmptyList()
    {
        using var db = new TrainingDiaryContext(_options);

        var result = await SummaryCalculator.BySportAsync(
            db.Workouts.AsNoTracking());

        Assert.Empty(result);
    }

    // ТЕСТ 2: однаковий вид спорту об'єднується.
    [Fact]
    public async Task BySport_SameSport_ReturnsOneGroup()
    {
        using var db = new TrainingDiaryContext(_options);

        db.Workouts.AddRange(
            CreateWorkout(
                "Біг",
                "Ранковий біг",
                20,
                new DateTime(2026, 11, 1)),

            CreateWorkout(
                "Біг",
                "Вечірній біг",
                30,
                new DateTime(2026, 11, 2))
        );

        await db.SaveChangesAsync();

        var result = await SummaryCalculator.BySportAsync(
            db.Workouts.AsNoTracking());

        var row = Assert.Single(result);

        Assert.Equal("Біг", row.Name);
        Assert.Equal(2, row.Count);
        Assert.Equal(50, row.Total);
    }

    // ТЕСТ 3: різні місяці не змішуються.
    [Fact]
    public async Task ByMonth_DifferentMonths_ReturnsSeparateGroups()
    {
        using var db = new TrainingDiaryContext(_options);

        db.Workouts.AddRange(
            CreateWorkout(
                "Біг",
                "Біг у жовтні",
                20,
                new DateTime(2026, 10, 10)),

            CreateWorkout(
                "Біг",
                "Біг у листопаді",
                40,
                new DateTime(2026, 11, 10))
        );

        await db.SaveChangesAsync();

        var result = await SummaryCalculator.ByMonthAsync(
            db.Workouts.AsNoTracking());

        Assert.Equal(2, result.Count);

        Assert.Equal("2026-10", result[0].Name);
        Assert.Equal(1, result[0].Count);
        Assert.Equal(20, result[0].Total);

        Assert.Equal("2026-11", result[1].Name);
        Assert.Equal(1, result[1].Count);
        Assert.Equal(40, result[1].Total);
    }

    // ТЕСТ 4: правильний загальний підсумок.
    [Fact]
    public async Task Overall_MultipleWorkouts_ReturnsCorrectTotal()
    {
        using var db = new TrainingDiaryContext(_options);

        db.Workouts.AddRange(
            CreateWorkout(
                "Біг",
                "Біг 1",
                20,
                new DateTime(2026, 11, 1)),

            CreateWorkout(
                "Плавання",
                "Плавання 1",
                30,
                new DateTime(2026, 11, 2)),

            CreateWorkout(
                "Силові",
                "Жим лежачи",
                40,
                new DateTime(2026, 11, 3))
        );

        await db.SaveChangesAsync();

        var result = await SummaryCalculator.OverallAsync(
            db.Workouts.AsNoTracking());

        Assert.Equal("Усього", result.Name);
        Assert.Equal(3, result.Count);
        Assert.Equal(90, result.Total);
    }
}
