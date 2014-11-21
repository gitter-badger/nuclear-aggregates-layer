using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient
{
    public class FinalStorageReplicateHotClientPerformedOperationsFlow : MessageFlowBase<FinalStorageReplicateHotClientPerformedOperationsFlow>,
                                                                         ISourceMessageFlow<PrimaryReplicateHotClientPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("9A7A0399-E5AE-4378-B4A8-B15852872D93"); }
        }

        public override string Description
        {
            get
            {
                return
                    "Маркер для потока выполненных операций в системе данные о которых хранят промежуточные результаты первичной обработки с конечной целью репликации горячих в MsCRM";
            }
        }
    }
}