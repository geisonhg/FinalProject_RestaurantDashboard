namespace RestaurantDashboard.Application.Sales.Dtos;

public sealed record SalesSummaryDto
{
    public DateOnly PeriodStart { get; init; }
    public DateOnly PeriodEnd { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TotalTips { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal NetProfit => TotalRevenue - TotalExpenses;
    public int TotalTransactions { get; init; }
    public decimal AverageOrderValue => TotalTransactions > 0
        ? Math.Round(TotalRevenue / TotalTransactions, 2) : 0m;
    public Dictionary<string, decimal> RevenueByPaymentMethod { get; init; } = new();
    public Dictionary<string, int> TransactionsByPaymentMethod { get; init; } = new();
    public Dictionary<string, decimal> TopSellingItems { get; init; } = new();
    public Dictionary<string, decimal> ExpensesByCategory { get; init; } = new();
}
