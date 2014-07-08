using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public sealed class AllPerformedOperationsFlow : MessageFlowBase<AllPerformedOperationsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("9A0272E1-F612-4B8C-87F5-AD56AD3C1A58"); }
        }

        public override string Description
        {
            get { return "Маркер для потока всех выполненных операций в системе"; }
        }
    }
}
