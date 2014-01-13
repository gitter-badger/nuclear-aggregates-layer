using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Model;

[assembly: ContainedTypes(
    typeof(IOrderValidationRule),
    typeof(IRequestHandler))]