﻿using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.Operations.DI
{
    public sealed class BlCoreOperationsAssembly : IZoneAssembly<OperationsZone>,
                                                   IZoneAnchor<OperationsZone>,
                                                   IContainsType<IAggregateReadModel>,
                                                   IContainsType<IOperation>,
                                                   IContainsType<IPersistenceService>,
                                                   IContainsType<IRequestHandler>,
                                                   IContainsType<ISimplifiedModelConsumer>
    {
    }
}