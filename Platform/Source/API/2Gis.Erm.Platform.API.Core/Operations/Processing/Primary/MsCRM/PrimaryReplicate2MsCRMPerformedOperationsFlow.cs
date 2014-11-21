using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM
{
    public sealed class PrimaryReplicate2MsCRMPerformedOperationsFlow : 
        MessageFlowBase<PrimaryReplicate2MsCRMPerformedOperationsFlow>, 
        IPerformedOperationsPrimaryProcessingFlow
    {
        public override Guid Id
        {
            get { return new Guid("DD0A14EE-3A56-41D1-AF85-6DC519A081FB"); }
        }

        public override string Description
        {
            get { return "Маркер для потока выполненных операций в системе первично обрабатываемых с конечной целью репликации в MsCRM"; }
        }
    }
}