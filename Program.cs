
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TrainingDiary.Api;
using TrainingDiary.Components;
using TrainingDiary.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// ЛР №13. КОНФІГУРАЦІЯ
// ==========================================

// Отримуємо рядок підключення з appsettings.json
var connectionString = builder.Configuration
    .GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Не знайдено ConnectionStrings:Default.");

// Власні параметри застосунку
var pageSize = builder.Configuration
    .GetValue<int>("TrainingDiary:PageSize", 5);

var applicationTitle = builder.Configuration[
    "TrainingDiary:Title"
] ?? "Щоденник тренувань";

if (pageSize < 1)
{
    throw new InvalidOperationException(
        "TrainingDiary:PageSize має бути більше нуля.");
}

// ==========================================
// СЕРВІСИ
// ==========================================

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SQLite
builder.Services.AddDbContextFactory<TrainingDiaryContext>(
    options => options.UseSqlite(connectionString));

// QuickGrid
builder.Services.AddQuickGridEntityFrameworkAdapter();

// Помилки Entity Framework Core
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// ЖУРНАЛЮВАННЯ
// ==========================================

app.Logger.LogInformation(
    "Запуск TrainingDiary. Назва: {Title}, " +
    "кількість записів на сторінці: {PageSize}",
    applicationTitle,
    pageSize);

// ==========================================
// АВТОМАТИЧНІ МІГРАЦІЇ
// ==========================================

// Міграції застосовуються до бази,
// вказаної в ConnectionStrings:Default.

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider
        .GetRequiredService<
            IDbContextFactory<TrainingDiaryContext>>();

    await using var db =
        await factory.CreateDbContextAsync();

    app.Logger.LogInformation(
        "Перевірка міграцій бази даних.");

    await db.Database.MigrateAsync();

    app.Logger.LogInformation(
        "Перевірку міграцій завершено.");
}

// ==========================================
// НАЛАШТУВАННЯ СЕРЕДОВИЩА
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();

    // Документація API в Development
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

// Статичні файли
app.MapStaticAssets();

// Сторінки Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ==========================================
// ЛР №11. МІНІМАЛЬНИЙ API
// ==========================================

var api = app.MapGroup("/api")
    .WithTags("TrainingDiary API");

// 1. Список тренувань
api.MapGet("/workouts",
    async (
        IDbContextFactory<TrainingDiaryContext> factory) =>
    {
        await using var db =
            await factory.CreateDbContextAsync();

        var workouts = await db.Workouts
            .AsNoTracking()
            .OrderBy(w => w.Id)
            .ToListAsync();

        var result = workouts
            .Select(w => new WorkoutDto(
                w.Id,
                w.SportType,
                w.ExerciseName,
                w.Type.ToString(),
                w.DurationMinutes,
                w.Distance,
                w.PerformedOn
            ))
            .ToList();

        return Results.Ok(result);
    })
    .WithName("GetWorkouts")
    .WithSummary("Отримати всі тренування")
    .WithDescription(
        "Повертає список тренувань у форматі JSON.")
    .Produces<List<WorkoutDto>>(
        StatusCodes.Status200OK);

// 2. Тренування за ID
api.MapGet("/workouts/{id:int}",
    async (
        int id,
        IDbContextFactory<TrainingDiaryContext> factory) =>
    {
        await using var db =
            await factory.CreateDbContextAsync();

        var workout = await db.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout is null)
        {
            return Results.NotFound();
        }

        var result = new WorkoutDto(
            workout.Id,
            workout.SportType,
            workout.ExerciseName,
            workout.Type.ToString(),
            workout.DurationMinutes,
            workout.Distance,
            workout.PerformedOn
        );

        return Results.Ok(result);
    })
    .WithName("GetWorkoutById")
    .WithSummary("Отримати тренування за ID")
    .WithDescription(
        "Повертає тренування або код 404, якщо запис не знайдено.")
    .Produces<WorkoutDto>(
        StatusCodes.Status200OK)
    .Produces(
        StatusCodes.Status404NotFound);

// 3. Загальне зведення
api.MapGet("/summary",
    async (
        IDbContextFactory<TrainingDiaryContext> factory) =>
    {
        await using var db =
            await factory.CreateDbContextAsync();

        var totalWorkouts =
            await db.Workouts.CountAsync();

        var totalMinutes =
            await db.Workouts.SumAsync(
                w => (int?)w.DurationMinutes) ?? 0;

        var averageMinutes = totalWorkouts == 0
            ? 0
            : Math.Round(
                (double)totalMinutes / totalWorkouts,
                2);

        var result = new SummaryRow(
            totalWorkouts,
            totalMinutes,
            averageMinutes
        );

        return Results.Ok(result);
    })
    .WithName("GetWorkoutSummary")
    .WithSummary("Отримати зведення тренувань")
    .WithDescription(
        "Повертає кількість тренувань, сумарну та середню тривалість.")
    .Produces<SummaryRow>(
        StatusCodes.Status200OK);

app.Run();
