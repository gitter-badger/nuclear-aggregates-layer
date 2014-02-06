using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.Model
{
    public class RutServiceSpecs
    {
        public abstract class RutServiceContext
        {
            protected static string _message;
            protected static bool? _isError;
            protected static string _rutNumber;
            protected static ICheckInnService _checkService;

            Establish context = () => _checkService = new ChileRutService();
            Because of = () => _isError = _checkService.TryGetErrorMessage(_rutNumber, out _message);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_totally_invalid_record : RutServiceContext
        {
            Establish context = () => _rutNumber = "123";
            It should_be_rejected = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_without_dots : RutServiceContext
        {
            Establish context = () => _rutNumber = "9608592-3";
            It should_be_rejected = () => _isError.Should().Be(true);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_with_checksum_K_in_lower_case : RutServiceContext
        {
            Establish context = () => _rutNumber = "24.758.479-k";
            It should_be_rejected = () => _isError.Should().Be(true); // Черт знает, чётких требований нет.
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_7_digit_rut : RutServiceContext
        {
            Establish context = () => _rutNumber = "9.608.592-3";
            It should_be_accepted = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_8_digit_rut : RutServiceContext
        {
            Establish context = () => _rutNumber = "15.518.774-3";
            It should_be_accepted = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_with_checksum_0 : RutServiceContext
        {
            Establish context = () => _rutNumber = "7.492.209-0";
            It should_be_accepted = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_with_checksum_K : RutServiceContext
        {
            Establish context = () => _rutNumber = "24.758.479-K";
            It should_be_accepted = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_with_checksum_1 : RutServiceContext
        {
            Establish context = () => _rutNumber = "9.576.728-1";
            It should_be_accepted = () => _isError.Should().Be(false);
        }

        [Tags("Model")]
        [Subject(typeof(ChileRutService))]
        class When_validating_valid_rut_with_checksum_9 : RutServiceContext
        {
            Establish context = () => _rutNumber = "15.944.430-9";
            It should_be_accepted = () => _isError.Should().Be(false);
        }
    }
}