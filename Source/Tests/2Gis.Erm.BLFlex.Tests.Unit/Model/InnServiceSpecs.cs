using DoubleGis.Erm.BL.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Unit.Tests.Model
{
    public class InnServiceSpecs
    {

        public abstract class InnServiceContext
        {
            protected static string _message;
            protected static bool _isError;

            Establish context = () => CheckInnService = new RussiaCheckInnService();

            protected static ICheckInnService CheckInnService { get; private set; }
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_10_symbols__0000000000_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("0000000000", out _message);
            It there_should_be_no_error = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_12_symbols_000000000000_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("000000000000", out _message);
            It there_should_be_no_error = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_3_symbols_000_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("000", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_15_symbols_000000000000000_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("000000000000000", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_xyz_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("xyz", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_some_random_symbols_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("12;\n$%\t", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_1234567890_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("1234567890", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_123456789012_value : InnServiceContext
        {
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("123456789012", out _message);
            It there_should_be_an_error = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_correct_3328461343_value : InnServiceContext
        {
            // примечание: номера взяты из интернета, доступны публично
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("3328461343", out _message);
            It there_should_be_no_error = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(RussiaCheckInnService))]
        class When_validating_INN_with_correct_500100732259_value : InnServiceContext
        {
            // примечание: номера взяты из интернета, доступны публично
            Because of = () => _isError = CheckInnService.TryGetErrorMessage("500100732259", out _message);
            It there_should_be_no_error = () => _isError.Should().Be(false);
        }
    }
}