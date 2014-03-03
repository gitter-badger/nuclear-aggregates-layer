using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;

[assembly: ContainedTypes(
    typeof(IAggregateReadModel),
    typeof(IOperation),
    typeof(IRequestHandler),
    typeof(ISimplifiedModelConsumer))]