using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM
{
    public sealed class FinalStorageReplicate2MsCRMPerformedOperationsFlow : MessageFlowBase<FinalStorageReplicate2MsCRMPerformedOperationsFlow>,
                                                                             ISourceMessageFlow<PrimaryReplicate2MsCRMPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("30E95163-315C-4C67-B9D5-2BF58072CE2D"); }
        }

        public override string Description
        {
            get
            {
                return "Маркер для потока выполненных операций в системе данные о которых хранят промежуточные результаты первичной обработки с конечной целью репликации в MsCRM";
            }
        }
    }
}