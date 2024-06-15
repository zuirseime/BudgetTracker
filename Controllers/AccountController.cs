using BudgetTracker.Models;
using BudgetTracker.Views.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    [Route("/account/")]
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        [Authorize]
        [Route("/account/{id?}")]
        public IActionResult Index(int id = 0)
        {
            var user = userManager.GetUserAsync(User).Result;
            ViewBag.UserName = user?.FullName;
            return View();
        }
        
        [Route("/account/login")]
        public IActionResult Login() {
            return View();
        }
        
        [Route("/account/login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded) {
                    var user = userManager.GetUserAsync(User).Result;
                    ViewBag.UserName = user?.FullName;

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
        
        [Route("/account/register")]
        public IActionResult Register() {
            return View();
        }
        
        [Route("/account/register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Login));
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [Route("/account/logout")]
        [HttpPost]
        public async Task<IActionResult> Logout() {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("/account/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}