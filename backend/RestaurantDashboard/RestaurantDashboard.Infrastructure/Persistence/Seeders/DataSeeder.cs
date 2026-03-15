using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.ValueObjects;
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
        await SeedHistoricalDataAsync(context);
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

    private static async Task SeedHistoricalDataAsync(AppDbContext context)
    {
        if (await context.Sales.AnyAsync()) return;

        var rng = new Random(42);
        var menuItems = await context.MenuItems.ToListAsync();
        var employeeIds = await context.Employees.Select(e => e.Id).ToListAsync();

        if (!menuItems.Any() || !employeeIds.Any()) return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = today.AddDays(-90);

        var paymentMethods = new[]
        {
            PaymentMethod.Card, PaymentMethod.Card, PaymentMethod.Card,
            PaymentMethod.Contactless, PaymentMethod.Contactless,
            PaymentMethod.Cash,
            PaymentMethod.GiftCard
        };

        // Reflection helpers to backdate private-set properties
        var saleDateProp = typeof(Sale).GetProperty(nameof(Sale.Date))!;
        var orderOpenedProp = typeof(Order).GetProperty(nameof(Order.OpenedAt))!;
        var orderClosedProp = typeof(Order).GetProperty(nameof(Order.ClosedAt))!;
        var tipDateProp = typeof(Tip).GetProperty(nameof(Tip.Date))!;

        var managerEmployeeId = employeeIds[0];

        for (var d = startDate; d <= today; d = d.AddDays(1))
        {
            bool isWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            int ordersForDay = isWeekend ? rng.Next(6, 11) : rng.Next(3, 8);

            for (int o = 0; o < ordersForDay; o++)
            {
                var empId = employeeIds[rng.Next(employeeIds.Count)];
                var order = Order.Open(rng.Next(1, 16), empId);

                foreach (var item in menuItems.OrderBy(_ => rng.Next()).Take(rng.Next(1, 5)))
                    order.AddItem(item.Id, item.Name, rng.Next(1, 4), Money.From(item.BasePrice.Amount));

                var paymentMethod = paymentMethods[rng.Next(paymentMethods.Length)];
                decimal tipAmount = rng.NextDouble() < 0.4
                    ? Math.Round(order.Subtotal.Amount * (decimal)rng.NextDouble() * 0.20m, 2)
                    : 0m;

                var sale = order.Close(paymentMethod, tipAmount);

                // Backdate to the historical day
                var closedAt = DateTime.SpecifyKind(
                    d.ToDateTime(new TimeOnly(rng.Next(11, 23), rng.Next(0, 60))),
                    DateTimeKind.Utc);
                var openedAt = closedAt.AddMinutes(-rng.Next(20, 75));

                orderOpenedProp.SetValue(order, openedAt);
                orderClosedProp.SetValue(order, (DateTime?)closedAt);
                saleDateProp.SetValue(sale, d);
                if (sale.Tip is not null)
                    tipDateProp.SetValue(sale.Tip, d);

                order.ClearDomainEvents();

                await context.Orders.AddAsync(order);
                await context.Sales.AddAsync(sale);
            }

            // Weekly restocking every Monday
            if (d.DayOfWeek == DayOfWeek.Monday)
            {
                var food = Expense.Record(ExpenseCategory.Food, rng.Next(200, 500), d,
                    "Weekly food stock purchase", managerEmployeeId);
                food.Approve();
                await context.Expenses.AddAsync(food);

                var bev = Expense.Record(ExpenseCategory.Beverage, rng.Next(80, 250), d,
                    "Weekly beverage restock", managerEmployeeId);
                bev.Approve();
                await context.Expenses.AddAsync(bev);
            }

            // Monthly fixed costs on the 1st
            if (d.Day == 1)
            {
                var utilities = Expense.Record(ExpenseCategory.Utilities, rng.Next(350, 700), d,
                    "Monthly utilities bill", managerEmployeeId);
                utilities.Approve();
                await context.Expenses.AddAsync(utilities);

                var wages = Expense.Record(ExpenseCategory.Wages, rng.Next(8000, 13000), d,
                    "Monthly staff wages", managerEmployeeId);
                wages.Approve();
                await context.Expenses.AddAsync(wages);

                var maint = Expense.Record(ExpenseCategory.Maintenance, rng.Next(100, 400), d,
                    "Monthly equipment maintenance", managerEmployeeId);
                maint.Approve();
                await context.Expenses.AddAsync(maint);
            }

            // Occasional marketing spend (~5 % of days)
            if (rng.NextDouble() < 0.05)
            {
                var mkt = Expense.Record(ExpenseCategory.Marketing, rng.Next(50, 300), d,
                    "Marketing and promotions", managerEmployeeId);
                mkt.Approve();
                await context.Expenses.AddAsync(mkt);
            }
        }

        await context.SaveChangesAsync();
    }
}
