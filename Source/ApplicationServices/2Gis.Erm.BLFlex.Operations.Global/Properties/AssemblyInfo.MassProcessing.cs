using System.Data.Services;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;

[assembly: ContainedTypes(
    typeof(IAggregateReadModel),
    typeof(IOperation),
    typeof(IRequestHandler))]