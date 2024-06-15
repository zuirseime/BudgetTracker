using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is requiered")] public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Icon is requiered")] public string Icon { get; set; } = null!;
    [NotMapped] public string FullTitle => $"{Icon} {Title}";

    public CategoryType Type { get; set; } = CategoryType.Expense;
}

public enum CategoryType {
    Expense, Income
}