
using System.ComponentModel.DataAnnotations;

namespace TrainingDiary.Models;

public class SportCategory
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Вкажіть назву категорії спорту")]
    [StringLength(60, ErrorMessage = "Назва категорії занадто довга")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Workout> Workouts { get; set; }
        = new List<Workout>();
}
