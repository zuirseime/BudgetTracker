using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Models;

public class Transaction
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public string UserId { get; set; } = null!;
    public User? User { get; set; }

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Precision(10, 2)]
    public double Amount { get; set; }

    public string? Note { get; set; }

    [NotMapped]
    public string? FormattedAmount => ((Category == null || Category.Type == CategoryType.Expense) ? "- " : "+ ") + Amount.ToString("C2");
}