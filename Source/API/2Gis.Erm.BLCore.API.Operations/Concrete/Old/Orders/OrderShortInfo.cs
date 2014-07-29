namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class OrderShortInfo
    {
        public OrderShortInfo(short releaseCountFact, int workflowStepId)
        {
            ReleaseCountFact = releaseCountFact;
            WorkflowStepId = workflowStepId;
        }

        public short ReleaseCountFact { get; private set; }
        public int WorkflowStepId { get; private set; }
    }
}