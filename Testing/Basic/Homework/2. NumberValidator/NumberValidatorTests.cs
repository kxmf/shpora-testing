
using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(0, Description = "Precision is zero")]
    [TestCase(-1, Description = "Precision is negative")]
    public void NumberValidator_ShouldThrowException_WhenPrecisionIsNonPositive(int value)
    {
        Action action = () => new NumberValidator(precision: value);

        action.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
    }

    [TestCase(4, -3, Description = "Scale is negative")]
    [TestCase(5, 5, Description = "Scale is equal to precision")]
    [TestCase(5, 7, Description = "Scale is greater than precision")]
    public void NumberValidator_ShouldThrowException_WhenScaleIsNegativeOrGreaterOrEqualToPrecision(
        int precision, int scale)
    {
        Action action = () => new NumberValidator(precision: precision, scale: scale);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("scale must be a non-negative number less than precision");
    }

    [TestCase(1, 0, true)]
    [TestCase(5, 2, false)]
    public void NumberValidator_ShouldNotThrowException_WhenArgumentsAreValid(
        int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().NotThrow();
    }

    [TestCase("", Description = "input is empty")]
    [TestCase(null, Description = "input is null")]
    public void IsValidNumber_ShouldReturnsFalse_WhenInputIsNullOrEmpty(string value)
    {
        var numberValidator = new NumberValidator(precision: 10, scale: 5);

        numberValidator.IsValidNumber(value).Should().Be(false);
    }

    [TestCase("abc", Description = "Letters instead of digits")]
    [TestCase("123.abc", Description = "Letters in fractional part")]
    [TestCase("a,bc", Description = "Letters with comma")]
    [TestCase("1.2.3", Description = "Multiple dots")]
    [TestCase("1.2,3", Description = "Mixed separators")]
    [TestCase(".12", Description = "Missing integer part")]
    [TestCase("12.", Description = "Missing fractional part after separator")]
    [TestCase("   ", Description = "Whitespace only")]
    [TestCase(" 12", Description = "Leading whitespace")]
    [TestCase("12 ", Description = "Trailing whitespace")]
    [TestCase("+-12", Description = "Multiple signs")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputDoesNotMatchFormat(string value)
    {
        var numberValidator = new NumberValidator(precision: 10, scale: 5);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("-123")]
    [TestCase("-1.23")]
    [TestCase("-0.00")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputIsNegativeAndOnlyPositiveIsTrue(string value)
    {
        var numberValidator = new NumberValidator(precision: 5, scale: 2, onlyPositive: true);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(3, 2, "1234", Description = "Integer part exceeds precision")]
    [TestCase(3, 2, "00.00", Description = "Total digits exceed precision")]
    [TestCase(17, 2, "0.000", Description = "Fractional part exceeds scale")]
    [TestCase(5, 0, "1.2", Description = "Fractional part is present when scale is zero")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputExceedsPrecisionOrScale(
        int precision, int scale, string value)
    {
        var numberValidator = new NumberValidator(precision: precision, scale: scale, onlyPositive: false);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(17, 2, true, "0", Description = "Single integer digit")]
    [TestCase(3, 0, true, "123", Description = "Integer matching exact precision")]
    [TestCase(17, 2, true, "0.0", Description = "Fraction with dot separator")]
    [TestCase(17, 2, true, "0,0", Description = "Fraction with comma separator")]
    [TestCase(4, 2, true, "+1.23", Description = "Positive sign with onlyPositive = true")]
    [TestCase(4, 2, false, "+1.23", Description = "Positive sign with onlyPositive = false")]
    [TestCase(4, 2, false, "-1.23", Description = "Negative number with onlyPositive = false")]
    public void IsValidNumber_ShouldReturnTrue_WhenInputIsValid(
        int precision, int scale, bool onlyPositive, string value)
    {
        var numberValidator = new NumberValidator(precision, scale, onlyPositive);

        numberValidator.IsValidNumber(value).Should().BeTrue();
    }
}