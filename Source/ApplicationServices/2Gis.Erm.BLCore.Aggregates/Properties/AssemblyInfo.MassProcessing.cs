using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;

[assembly: ContainedTypes(
    typeof(IAggregateReadModel), 
    typeof(IAggregateRepository),
    typeof(ISimplifiedModelConsumerReadModel))]