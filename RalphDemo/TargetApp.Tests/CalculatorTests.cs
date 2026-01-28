using Xunit;
using TargetApp;

namespace TargetApp.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        var calculator = new Calculator();
        var result = calculator.Add(5, 3);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Multiply_ReturnsProduct()
    {
        var calculator = new Calculator();
        var result = calculator.Multiply(4, 3);
        Assert.Equal(12, result);
    }
}
