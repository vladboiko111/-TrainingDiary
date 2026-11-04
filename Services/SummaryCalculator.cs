
using Microsoft.EntityFrameworkCore;
using TrainingDiary.Models;

namespace TrainingDiary.Services;

// Рядок підсумкової таблиці.
public record SummaryRow(
    string Name,
    int Count,
    int Total
);

public static class SummaryCalculator
{
    // 1. Групування тренувань за видом спорту.
    public static async Task<List<SummaryRow>> BySportAsync(
        IQueryable<Workout> query)
    {
        // Групування й обчислення виконує SQLite.
        var raw = await query
            .GroupBy(w => w.SportType)
            .Select(g => new
            {
                Name = g.Key,
                Count = g.Count(),
                Total = g.Sum(w => w.DurationMinutes)
            })
            .OrderBy(r => r.Name)
            .ToListAsync();

        // Перетворення на SummaryRow після SQL-запиту.
        return raw
            .Select(r => new SummaryRow(
                r.Name,
                r.Count,
                r.Total
            ))
            .ToList();
    }

    // 2. Групування за роком і місяцем.
    public static async Task<List<SummaryRow>> ByMonthAsync(
        IQueryable<Workout> query)
    {
        var raw = await query
            .GroupBy(w => new
            {
                Year = w.PerformedOn.Year,
                Month = w.PerformedOn.Month
            })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count(),
                Total = g.Sum(w => w.DurationMinutes)
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToListAsync();

        return raw
            .Select(r => new SummaryRow(
                $"{r.Year}-{r.Month:D2}",
                r.Count,
                r.Total
            ))
            .ToList();
    }

    // 3. Загальні показники.
    public static async Task<SummaryRow> OverallAsync(
        IQueryable<Workout> query)
    {
        // Обчислюємо загальну кількість і тривалість у SQL.
        var result = await query
            .GroupBy(w => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Total = g.Sum(w => w.DurationMinutes)
            })
            .FirstOrDefaultAsync();

        // Якщо таблиця порожня, повертаємо нулі.
        return result is null
            ? new SummaryRow("Усього", 0, 0)
            : new SummaryRow(
                "Усього",
                result.Count,
                result.Total
            );
    }
}
