namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class ImportFlowStickersIdentity : OperationIdentityBase<ImportFlowStickersIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportFlowStickersIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт горячих клиентов";
            }
        }
    }
}