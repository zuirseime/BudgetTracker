using BudgetTracker.Models;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BudgetTracker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("DevString");
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

using (var scope = app.Services.CreateScope()) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = [ Globals.AdminRole, Globals.UserRole ];

    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

// using (var scope = app.Services.CreateScope()) {
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

//     var email = "mail@example.com";
    
//     if (await userManager.FindByEmailAsync(email) is User user) {
//         if (!await userManager.IsInRoleAsync(user, Globals.AdminRole)) {
//             await userManager.AddToRoleAsync(user, Globals.AdminRole);
//         }
//     }
// }

app.Run();
