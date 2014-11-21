﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationConsistencyVerifier
    {
        bool IsOperationContextConsistent(IEnumerable<IVerifierContext> verifierContexts);
    }
}
