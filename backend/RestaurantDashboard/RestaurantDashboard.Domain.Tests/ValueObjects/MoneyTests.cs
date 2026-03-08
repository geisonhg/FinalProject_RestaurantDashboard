using FluentAssertions;
using RestaurantDashboard.Domain.Exceptions;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Tests.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void From_WithPositiveAmount_CreatesMoneyWithCorrectAmount()
    {
        var money = Money.From(100m);

        money.Amount.Should().Be(100m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void From_WithNegativeAmount_ThrowsDomainException()
    {
        var act = () => Money.From(-1m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Add_TwoMoneyValues_ReturnsSumAmount()
    {
        var a = Money.From(10m);
        var b = Money.From(20m);

        var result = a + b;

        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Subtract_SmallerFromLarger_ReturnsCorrectDifference()
    {
        var a = Money.From(50m);
        var b = Money.From(20m);

        var result = a - b;

        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Multiply_MoneyByScalar_ReturnsCorrectProduct()
    {
        var money = Money.From(15m);

        var result = money * 3;

        result.Amount.Should().Be(45m);
    }

    [Fact]
    public void Zero_ReturnsMoneyWithZeroAmount()
    {
        var zero = Money.Zero();

        zero.Amount.Should().Be(0m);
    }

    [Fact]
    public void TwoMoneyWithSameAmount_AreEqual()
    {
        var a = Money.From(42m);
        var b = Money.From(42m);

        a.Should().Be(b);
    }
}
