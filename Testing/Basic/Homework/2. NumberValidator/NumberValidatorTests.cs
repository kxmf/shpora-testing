using NUnit.Framework;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(0, TestName = "Precision is zero")]
    [TestCase(-1, TestName = "Precision is negative")]
    public void NumberValidator_ShouldThrowException_WhenPrecisionIsInvalid(int value)
    {
        Action action = () => new NumberValidator(precision: value);

        action.Should().Throw<ArgumentException>();
    }

    [TestCase(4, -1, TestName = "Scale is negative")]
    [TestCase(5, 5, TestName = "Scale is equal to precision")]
    [TestCase(5, 7, TestName = "Scale is greater than precision")]
    public void NumberValidator_ShouldThrowException_WhenScaleIsInvalid(
        int precision, int scale)
    {
        Action action = () => new NumberValidator(precision: precision, scale: scale);

        action.Should().Throw<ArgumentException>();
    }

    [TestCase(1, 0, true, TestName = "Scale is zero and onlyPositive is true")]
    [TestCase(5, 2, false, TestName = "Scale is positive and onlyPositive is false")]
    public void NumberValidator_ShouldNotThrowException_WhenArgumentsAreValid(
        int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().NotThrow();
    }

    [TestCase(null, TestName = "Input is null")]
    [TestCase("", TestName = "Input is empty")]
    [TestCase("   ", TestName = "Input is whitespace only")]
    [TestCase(" 12", TestName = "Leading whitespace")]
    [TestCase("12 ", TestName = "Trailing whitespace")]
    [TestCase("abc", TestName = "Letters instead of digits")]
    [TestCase("123.abc", TestName = "Letters in fractional part")]
    [TestCase("a,bc", TestName = "Letters in both parts with comma")]
    [TestCase("1.2.3", TestName = "Multiple dots")]
    [TestCase("1.2,3", TestName = "Mixed separators")]
    [TestCase(".12", TestName = "Missing integer part")]
    [TestCase("12.", TestName = "Missing fractional part after separator")]
    [TestCase("+-12", TestName = "Multiple signs")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputIsInvalid(string value)
    {
        var numberValidator = new NumberValidator(precision: 10, scale: 5);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("-123", TestName = "Negative integer when onlyPositive is true")]
    [TestCase("-1.23", TestName = "Negative fraction when onlyPositive is true")]
    [TestCase("-0.00", TestName = "Negative zero when onlyPositive is true")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputIsNegativeAndOnlyPositiveIsTrue(string value)
    {
        var numberValidator = new NumberValidator(precision: 5, scale: 2, onlyPositive: true);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(3, 2, "1234", TestName = "Integer part length exceeds precision")]
    [TestCase(3, 2, "00.00", TestName = "Total digits count exceeds precision")]
    [TestCase(17, 2, "0.000", TestName = "Fractional part length exceeds scale")]
    [TestCase(5, 0, "1.2", TestName = "Fractional part is present when scale is zero")]
    [TestCase(3, 2, "+1.23", TestName = "Total length with plus sign exceeds precision")]
    [TestCase(3, 2, "-1.23", TestName = "Total length with minus sign exceeds precision")]
    public void IsValidNumber_ShouldReturnFalse_WhenInputExceedsPrecisionOrScale(
        int precision, int scale, string value)
    {
        var numberValidator = new NumberValidator(precision: precision, scale: scale, onlyPositive: false);

        numberValidator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(17, 2, true, "0", TestName = "Single digit integer")]
    [TestCase(3, 0, true, "123", TestName = "Integer length equals exact precision")]
    [TestCase(17, 2, true, "0.0", TestName = "Fraction with dot separator")]
    [TestCase(17, 2, true, "0,0", TestName = "Fraction with comma separator")]
    [TestCase(4, 2, true, "+1.23", TestName = "Length with plus sign equals exact precision when onlyPositive is true")]
    [TestCase(4, 2, false, "+1.23", TestName = "Length with plus sign equals exact precision when onlyPositive is false")]
    [TestCase(4, 2, false, "-1.23", TestName = "Length with minus sign equals exact precision when onlyPositive is false")]
    public void IsValidNumber_ShouldReturnTrue_WhenInputIsValid(
        int precision, int scale, bool onlyPositive, string value)
    {
        var numberValidator = new NumberValidator(precision, scale, onlyPositive);

        numberValidator.IsValidNumber(value).Should().BeTrue();
    }
}