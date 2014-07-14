using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch
{
    public sealed class PrimaryReplicate2ElasticSearchPerformedOperationsFlow : MessageFlowBase<PrimaryReplicate2ElasticSearchPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("2906DF54-CCAD-4296-AF74-1C3AC6D5F99B"); }
        }

        public override string Description
        {
            get { return "Маркер для потока выполненных операций в системе первично обрабатываемых с конечной целью репликации в ElasticSearch"; }
        }
    }
}