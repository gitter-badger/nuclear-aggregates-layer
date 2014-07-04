﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public interface ITrackedUseCase2PerfomedBusinessOperationsConverter
    {
        IEnumerable<PerformedBusinessOperation> Convert(TrackedUseCase trackedUseCase);
    }
}