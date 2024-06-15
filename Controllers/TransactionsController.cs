using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Controllers
{
    [Route("/transactions")]
    public class TransactionsController(DataContext context, UserManager<User> userManager) : Controller
    {
        [Route("/transactions")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var transactions = context.Transactions.Where(u => u.UserId == userManager.GetUserId(User)).Include(t => t.Category);
            return transactions != null ?
                        View(await transactions.ToListAsync()) :
                        Problem("Entity set 'DataContext.Categories' is null.");
        }

        
        [Route("/transactions/entry/{id?}")]
        [Authorize]
        public async Task<IActionResult> Entry(int id = 0) {
            await PopulateCategories();

            Transaction? transaction = new();
            if (id != 0) transaction = await context.Transactions.FindAsync(id);
            return View(transaction);
        }

        [Route("/transactions/entry/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Entry([Bind("Id,CategoryId,UserId,Date,Amount,Note")] Transaction transaction, int id = 0) {
            if (ModelState.IsValid) {
                if (id == 0) await context.AddAsync(transaction);
                else context.Update(transaction);

                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Globals.AdminRole)]
        public async Task<IActionResult> Delete(int id) {
            if (context.Categories == null)
            {
                return Problem("Entity set 'DataContext.Transactions' is null.");
            }
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private async Task PopulateCategories() {
            var categories = await context.Categories.ToListAsync();
            Category defaultCategory = new() { Id = 0, Title = "Choose a category" };
            categories.Insert(0, defaultCategory);
            ViewBag.Categories = categories;
        }
    }
}