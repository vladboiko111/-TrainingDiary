
using Microsoft.EntityFrameworkCore;
using TrainingDiary.Models;

namespace TrainingDiary.Data;

public class TrainingDiaryContext : DbContext
{
    public TrainingDiaryContext(
        DbContextOptions<TrainingDiaryContext> options)
        : base(options)
    {
    }

    public DbSet<Workout> Workouts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування збереження Distance у SQLite
        modelBuilder.Entity<Workout>()
            .Property(w => w.Distance)
            .HasConversion<double?>();

        // Унікальний індекс для назви вправи та дати
        modelBuilder.Entity<Workout>()
            .HasIndex(w => new
            {
                w.ExerciseName,
                w.PerformedOn
            })
            .IsUnique();
    }
}
