namespace RestaurantDashboard.Application.Reports.Commands.ImportExpensesFromCsv;

/// <summary>
/// Represents one row in the CSV file.
/// Expected columns: Date, Category, Amount, Description
/// Category must match <see cref="RestaurantDashboard.Domain.Enums.ExpenseCategory"/> names
/// (Food, Beverage, Utilities, Wages, Maintenance, Marketing, Other).
/// </summary>
public sealed class ExpenseCsvRow
{
    public DateOnly Date { get; set; }
    public string Category { get; set; } = "Other";
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
