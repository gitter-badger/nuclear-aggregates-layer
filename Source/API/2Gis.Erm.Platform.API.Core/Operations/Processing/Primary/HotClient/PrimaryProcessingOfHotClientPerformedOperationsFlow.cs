using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient
{
    public class PrimaryProcessingOfHotClientPerformedOperationsFlow : MessageFlowBase<PrimaryProcessingOfHotClientPerformedOperationsFlow>, 
                                                                       IPerformedOperationsPrimaryProcessingFlow
    {
        public override Guid Id
        {
            get
            {
                return new Guid("451DD85A-4BB8-42CA-87B5-76C1B580587E");
            }
        }

        public override string Description
        {
            get
            {
                return "Маркер для потока выполненных операций в системе, первично отслеживаемых с конечной целью обработки запросов по горячим клиентам.";
            }
        }
    }
}