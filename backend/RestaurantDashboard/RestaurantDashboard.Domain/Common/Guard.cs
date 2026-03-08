using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"'{paramName}' cannot be null or empty.");
    }

    public static void AgainstNegative(decimal value, string paramName)
    {
        if (value < 0)
            throw new DomainException($"'{paramName}' cannot be negative. Value: {value}");
    }

    public static void AgainstNegativeOrZero(int value, string paramName)
    {
        if (value <= 0)
            throw new DomainException($"'{paramName}' must be greater than zero. Value: {value}");
    }

    public static void AgainstOrderClosed(OrderStatus status)
    {
        if (status != OrderStatus.Open)
            throw new DomainException($"Operation is not permitted on an order with status '{status}'.");
    }

    public static void AgainstOrderAlreadyClosed(OrderStatus status)
    {
        if (status == OrderStatus.Closed || status == OrderStatus.Voided)
            throw new DomainException($"Order is already '{status}' and cannot be modified.");
    }

    public static void AgainstPastDate(DateOnly date, string paramName)
    {
        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException($"'{paramName}' cannot be a future date.");
    }
}
