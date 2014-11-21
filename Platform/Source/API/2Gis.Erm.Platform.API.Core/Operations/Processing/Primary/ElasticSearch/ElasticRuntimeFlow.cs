using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch
{
    public sealed class ElasticRuntimeFlow : MessageFlowBase<ElasticRuntimeFlow>
    {
        public override Guid Id
        {
            get { return new Guid("F23F460C-717B-4571-ABBB-3C0706DAF96F"); }
        }

        public override string Description
        {
            get { return ""; }
        }
    }
}