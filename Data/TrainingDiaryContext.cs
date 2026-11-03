
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

    // Основна таблиця тренувань
    public DbSet<Workout> Workouts { get; set; } = null!;

    // Довідник категорій спорту
    public DbSet<SportCategory> SportCategories { get; set; } = null!;

    // Довідник вправ
    public DbSet<Exercise> Exercises { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Збереження дистанції в SQLite
        modelBuilder.Entity<Workout>()
            .Property(w => w.Distance)
            .HasConversion<double?>();

        // Унікальний індекс із лабораторної №6
        modelBuilder.Entity<Workout>()
            .HasIndex(w => new
            {
                w.ExerciseName,
                w.PerformedOn
            })
            .IsUnique();

        // Зв'язок категорії спорту з тренуваннями
        modelBuilder.Entity<Workout>()
            .HasOne(w => w.SportCategory)
            .WithMany(c => c.Workouts)
            .HasForeignKey(w => w.SportCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Зв'язок вправи з тренуваннями
        modelBuilder.Entity<Workout>()
            .HasOne(w => w.Exercise)
            .WithMany(e => e.Workouts)
            .HasForeignKey(w => w.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
