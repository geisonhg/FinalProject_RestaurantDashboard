using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Infrastructure.Identity;

namespace RestaurantDashboard.Infrastructure.Persistence.Seeders;

public static class DataSeeder
{
    private static readonly string[] RoleNames = ["Admin", "Manager", "Staff"];

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        // MigrateAsync is a no-op for InMemory databases; skip it to avoid exceptions.
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager, context);
        await SeedMenuItemsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
        }
    }

    private static async Task SeedUsersAsync(
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        if (await userManager.Users.AnyAsync()) return;

        var seeds = new[]
        {
            (Email: "admin@restaurant.com",   Password: "Admin@12345!",   Role: "Admin",   EmpRole: EmployeeRole.Admin,   First: "Admin",  Last: "User"),
            (Email: "manager@restaurant.com", Password: "Manager@12345!", Role: "Manager", EmpRole: EmployeeRole.Manager, First: "Sarah",  Last: "Connor"),
            (Email: "staff@restaurant.com",   Password: "Staff@12345!",   Role: "Staff",   EmpRole: EmployeeRole.Waiter,  First: "John",   Last: "Smith")
        };

        foreach (var seed in seeds)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = seed.Email,
                Email = seed.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, seed.Password);
            if (!result.Succeeded) continue;

            await userManager.AddToRoleAsync(user, seed.Role);

            var employee = Employee.Create(seed.First, seed.Last, seed.EmpRole, DateOnly.FromDateTime(DateTime.UtcNow));
            employee.LinkToUser(user.Id);
            await context.Employees.AddAsync(employee);
            user.EmployeeId = employee.Id;
            await userManager.UpdateAsync(user);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedMenuItemsAsync(AppDbContext context)
    {
        if (await context.MenuItems.AnyAsync()) return;

        var items = new[]
        {
            ("Beef Burger",      "Mains",   14.50m),
            ("Caesar Salad",     "Starters", 8.50m),
            ("Fish & Chips",     "Mains",   16.00m),
            ("Garlic Bread",     "Starters", 4.50m),
            ("Chocolate Mousse", "Desserts", 6.50m),
            ("Cheesecake",       "Desserts", 6.00m),
            ("Pint of Guinness", "Drinks",   6.50m),
            ("House Wine",       "Drinks",   7.00m),
            ("Sparkling Water",  "Drinks",   2.50m),
            ("Chicken Wings",    "Starters", 9.50m),
        };

        foreach (var (name, category, price) in items)
            await context.MenuItems.AddAsync(MenuItem.Create(name, category, price));

        await context.SaveChangesAsync();
    }
}
