using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient
{
    public class FinalProcessingOfHotClientPerformedOperationsFlow : MessageFlowBase<FinalProcessingOfHotClientPerformedOperationsFlow>, 
                                                                     ISourceMessageFlow<FinalStorageProcessingOfHotClientPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get
            {
                return new Guid("305EA599-EA56-4DB2-BE38-FA86D88AB76D");
            }
        }

        public override string Description
        {
            get
            {
                return "Маркер для потока обработки запросов по горячим клиентам, выполняемой при окончательной обработке произведенных операций";
            }
        }
    }
}