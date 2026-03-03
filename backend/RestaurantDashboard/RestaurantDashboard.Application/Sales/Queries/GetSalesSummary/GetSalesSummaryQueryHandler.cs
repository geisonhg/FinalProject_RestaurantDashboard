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
        var salesTask   = _sales.GetByDateRangeAsync(request.From, request.To, cancellationToken);
        var expenseTask = _expenses.GetTotalAsync(request.From, request.To, cancellationToken);

        await Task.WhenAll(salesTask, expenseTask);

        var saleRecords = salesTask.Result;
        var totalExpenses = expenseTask.Result;

        var revenueByPayment = saleRecords
            .GroupBy(s => s.PaymentMethod)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(s => s.TotalAmount.Amount));

        var txByPayment = saleRecords
            .GroupBy(s => s.PaymentMethod)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new SalesSummaryDto
        {
            PeriodStart                  = request.From,
            PeriodEnd                    = request.To,
            TotalRevenue                 = saleRecords.Sum(s => s.TotalAmount.Amount),
            TotalTips                    = saleRecords.Sum(s => s.TipAmount.Amount),
            TotalTransactions            = saleRecords.Count,
            TotalExpenses                = totalExpenses,
            RevenueByPaymentMethod       = revenueByPayment,
            TransactionsByPaymentMethod  = txByPayment
        };
    }
}
