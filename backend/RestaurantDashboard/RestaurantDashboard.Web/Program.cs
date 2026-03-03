using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Application;
using RestaurantDashboard.Infrastructure;
using RestaurantDashboard.Infrastructure.Identity;
using RestaurantDashboard.Infrastructure.Persistence;
using RestaurantDashboard.Infrastructure.Persistence.Seeders;
using RestaurantDashboard.Web.Components;
using Microsoft.AspNetCore.Identity.UI.Services;
using RestaurantDashboard.Web.Components.Account;
using RestaurantDashboard.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Detect if a real Postgres connection is configured
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var useInMemory = connectionString.Contains("yourpassword") || string.IsNullOrWhiteSpace(connectionString);

if (useInMemory)
{
    // Preview mode — InMemory database, no Postgres required
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("RestaurantDashboardPreview"));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 10;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddInfrastructureRepositories();
}
else
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

// Blazor Identity UI services (wires up IdentityRedirectManager, etc.)
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddApplication();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<DashboardStateService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("ManagerPlus", p => p.RequireRole("Admin", "Manager"));
    options.AddPolicy("CanGenerateReports", p => p.RequireRole("Admin", "Manager"));
    options.AddPolicy("AllStaff", p => p.RequireAuthenticatedUser());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Wire up Blazor Identity endpoint routes (logout, external login, etc.)
app.MapAdditionalIdentityEndpoints();

// Seed database on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await DataSeeder.SeedAsync(context, userManager, roleManager);
}

app.Run();
