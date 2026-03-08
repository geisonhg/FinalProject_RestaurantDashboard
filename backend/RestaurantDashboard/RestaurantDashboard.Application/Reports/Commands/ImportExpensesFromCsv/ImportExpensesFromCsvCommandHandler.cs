using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Reports.Commands.ImportExpensesFromCsv;

public sealed class ImportExpensesFromCsvCommandHandler
    : IRequestHandler<ImportExpensesFromCsvCommand, int>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _uow;

    public ImportExpensesFromCsvCommandHandler(IExpenseRepository expenses, IUnitOfWork uow)
    {
        _expenses = expenses;
        _uow = uow;
    }

    public async Task<int> Handle(ImportExpensesFromCsvCommand request, CancellationToken cancellationToken)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
        };

        using var stream = new MemoryStream(request.CsvContent);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        var rows = csv.GetRecords<ExpenseCsvRow>().ToList();
        int count = 0;

        foreach (var row in rows)
        {
            if (row.Amount <= 0 || string.IsNullOrWhiteSpace(row.Description))
                continue;

            if (!Enum.TryParse<ExpenseCategory>(row.Category, ignoreCase: true, out var category))
                category = ExpenseCategory.Other;

            var expense = Expense.Record(
                category,
                row.Amount,
                row.Date,
                row.Description.Trim(),
                request.RecordedByEmployeeId);

            await _expenses.AddAsync(expense, cancellationToken);
            count++;
        }

        if (count > 0)
            await _uow.CommitAsync(cancellationToken);

        return count;
    }
}
