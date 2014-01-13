﻿using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
    public interface IDisqualifyApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(DisqualifyOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
        DisqualifyResult Execute(EntityName entityName, long entityId, bool? bypassValidation);
    }
}