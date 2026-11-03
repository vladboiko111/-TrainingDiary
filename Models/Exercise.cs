
using System.ComponentModel.DataAnnotations;

namespace TrainingDiary.Models;

public class Exercise
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Вкажіть назву вправи")]
    [StringLength(100, ErrorMessage = "Назва вправи занадто довга")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Workout> Workouts { get; set; }
        = new List<Workout>();
}
