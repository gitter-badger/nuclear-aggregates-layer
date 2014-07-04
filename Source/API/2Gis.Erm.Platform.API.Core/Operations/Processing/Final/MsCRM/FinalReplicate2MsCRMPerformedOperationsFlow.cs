using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM
{
    public sealed class FinalReplicate2MsCRMPerformedOperationsFlow : MessageFlowBase<FinalReplicate2MsCRMPerformedOperationsFlow>,
                                                                      ISourceMessageFlow<FinalStorageReplicate2MsCRMPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("C50202B0-4066-433B-A89D-1ED5EC97D161"); }
        }

        public override string Description
        {
            get { return "Маркер для потока репликации в MsCRM, выполняемой при окончательной обработке произведенных операций"; }
        }
    }
}