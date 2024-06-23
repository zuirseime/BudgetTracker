using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Controllers
{
    [Route("[controller]")]
    public class DashboardController(DataContext context, UserManager<User> userManager) : Controller
    {
        [Route("/[controller]/{id?}")]
        public async Task<IActionResult> Index(int id = 0)
        {
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-6));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today);
            User user = userManager.GetUserAsync(User)?.Result!;
            List<Transaction> transactions = await context.Transactions.Include(x => x.Category).Where(t 
                => t.UserId == user.Id && t.Date >= startDate && t.Date <= endDate
            ).ToListAsync();

            double totalIncome = transactions.Where(t => t.Category.Type == CategoryType.Income).Sum(t => t.Amount);
            ViewBag.TotalIncome = totalIncome.ToString("C2");

            double totalExpense = transactions.Where(t => t.Category.Type == CategoryType.Expense).Sum(t => t.Amount);
            ViewBag.TotalExpense = totalExpense.ToString("C2");

            double balance = totalIncome - totalExpense;
            ViewBag.Balance = balance.ToString("C2");

            return View();
        }
    }
}