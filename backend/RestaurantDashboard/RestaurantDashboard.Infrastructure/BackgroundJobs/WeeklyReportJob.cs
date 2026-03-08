using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantDashboard.Application.Reports.Commands.GenerateWeeklyReport;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.BackgroundJobs;

/// <summary>
/// Runs once every Monday at 03:00 UTC to auto-generate the previous week's report.
/// Also runs once 60 seconds after startup so you can see it working immediately in dev.
/// </summary>
public sealed class WeeklyReportJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<WeeklyReportJob> _logger;

    public WeeklyReportJob(IServiceProvider services, ILogger<WeeklyReportJob> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Short delay so the app finishes starting up before we touch the DB
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await TryGenerateAsync(stoppingToken);

                // Wait until next Monday 03:00 UTC
                var delay = TimeUntilNextMonday();
                _logger.LogInformation("WeeklyReportJob: next run in {Hours:F1} hours.", delay.TotalHours);
                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("WeeklyReportJob: stopped gracefully.");
        }
    }

    private async Task TryGenerateAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var employees = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();

            var allEmployees = await employees.GetAllActiveAsync(ct);
            var systemEmployee = allEmployees.FirstOrDefault();

            if (systemEmployee is null)
            {
                _logger.LogWarning("WeeklyReportJob: no active employees found — skipping.");
                return;
            }

            var to = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var from = to.AddDays(-7);

            var reportId = await mediator.Send(new GenerateWeeklyReportCommand
            {
                From = from,
                To = to,
                GeneratedByEmployeeId = systemEmployee.Id,
                Type = ReportType.Weekly,
            }, ct);

            _logger.LogInformation(
                "WeeklyReportJob: report {ReportId} generated for {From} — {To}.",
                reportId, from, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WeeklyReportJob: failed to generate report.");
        }
    }

    private static TimeSpan TimeUntilNextMonday()
    {
        var now = DateTime.UtcNow;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7;
        var nextMonday = now.Date.AddDays(daysUntilMonday).AddHours(3);
        return nextMonday - now;
    }
}
