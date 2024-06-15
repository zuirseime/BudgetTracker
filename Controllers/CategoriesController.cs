using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Controllers
{
    [Route("/categories")]
    public class CategoriesController(DataContext context) : Controller
    {
        [Route("/categories")]
        public async Task<IActionResult> Index()
        {
            return context.Categories != null ?
                        View(await context.Categories.ToListAsync()) :
                        Problem("Entity set 'DataContext.Categories' is null.");
        }

        
        [Route("/categories/entry/{id?}")]
        [Authorize(Roles = Globals.AdminRole)]
        public async Task<IActionResult> Entry(int id = 0) {
            Category? category = new();
            if (id != 0) category = await context.Categories.FindAsync(id);
            return View(category);
        }

        [Route("/categories/entry/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Globals.AdminRole)]
        public async Task<IActionResult> Entry([Bind("Id,Title,Icon,Type")] Category category, int id = 0) {
            if (ModelState.IsValid) {
                if (id == 0) await context.AddAsync(category);
                else context.Update(category);

                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Globals.AdminRole)]
        public async Task<IActionResult> Delete(int id) {
            if (context.Categories == null)
            {
                return Problem("Entity set 'DataContext.Categories' is null.");
            }
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}