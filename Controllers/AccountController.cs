using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Views.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    [Route("/account/")]
    public class AccountController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        // [Authorize]
        [Route("/account/{username?}")]
        public IActionResult Index(string? username = null)
        {
            if (string.IsNullOrEmpty(username)) {
                if (User.Identity is null)
                    return NotFound();
                
                username = User.Identity.Name;
                return RedirectToAction(nameof(Index), new { username });
            }

            var user = userManager.FindByNameAsync(username).Result;
            ViewBag.IsPersonal = User.Identity?.Name == username;
            return View(user);
        }

        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/account/upload-image/")]
        public async Task<IActionResult> UploadImage(string username, IFormFile image, string old) {
            if (image is null || image.Length == 0) return BadRequest();

            var fileName = Path.GetFileName(image.FileName);
            var filePath = Path.Combine(Globals.ImageFolder, fileName);
            userManager.GetUserAsync(User).Result!.ImageURI = fileName;

            using (FileStream create = new(filePath, FileMode.Create)) {
                await image.CopyToAsync(create);
            }

            if (old != Globals.DefaultPicture) {
                var oldPath = Globals.GetUserPic(old, false);
                if (System.IO.File.Exists(oldPath)) {
                    System.IO.File.Delete(oldPath);
                }
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { username });
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/account/change-user-name/")]
        public async Task<IActionResult> ChangeUserName(string username) {
            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null) {
                ModelState.AddModelError(string.Empty, "UserName is already taken.");
                return View();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.UserName = username;
            user.NormalizedUserName = userManager.NormalizeName(username);
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

            await signInManager.RefreshSignInAsync(user);
            return RedirectToAction(nameof(Index), new { username });
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
                string username = userManager.FindByEmailAsync(model.Email).Result?.UserName!;
                var result = await signInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, lockoutOnFailure: false);
                
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