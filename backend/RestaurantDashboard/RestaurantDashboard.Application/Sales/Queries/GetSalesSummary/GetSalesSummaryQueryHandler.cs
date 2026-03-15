using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;
namespace RestaurantDashboard.Application.Sales.Queries.GetSalesSummary;
public sealed class GetSalesSummaryQueryHandler
    : IRequestHandler<GetSalesSummaryQuery, SalesSummaryDto>
{
    private readonly ISaleRepository _sales;
    private readonly IExpenseRepository _expenses;
    public GetSalesSummaryQueryHandler(ISaleRepository sales, IExpenseRepository expenses)
    {
        _sales = sales;
        _expenses = expenses;
    }
    public async Task<SalesSummaryDto> Handle(
        GetSalesSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // Sequential — EF Core DbContext is not thread-safe
        var saleRecords = await _sales.GetByDateRangeAsync(request.From, request.To, cancellationToken);
        var totalExpenses = await _expenses.GetTotalAsync(request.From, request.To, cancellationToken);
        var topItems = await _sales.GetTopSellingItemsAsync(request.From, request.To, cancellationToken);
        var expenseRecords = await _expenses.GetByDateRangeAsync(request.From, request.To, cancellationToken);

        var revenueByPayment = saleRecords
            .GroupBy(s => s.PaymentMethod)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(s => s.TotalAmount.Amount));
        var txByPayment = saleRecords
            .GroupBy(s => s.PaymentMethod)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());
        var expensesByCategory = expenseRecords
            .Where(e => e.IsApproved)
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(e => e.Amount.Amount));

        return new SalesSummaryDto
        {
            PeriodStart = request.From,
            PeriodEnd = request.To,
            TotalRevenue = saleRecords.Sum(s => s.TotalAmount.Amount),
            TotalTips = saleRecords.Sum(s => s.TipAmount.Amount),
            TotalTransactions = saleRecords.Count,
            TotalExpenses = totalExpenses,
            RevenueByPaymentMethod = revenueByPayment,
            TransactionsByPaymentMethod = txByPayment,
            TopSellingItems = topItems,
            ExpensesByCategory = expensesByCategory
        };
    }
}
