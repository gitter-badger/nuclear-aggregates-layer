namespace DoubleGis.Erm.Model.Metadata.Operations.Identity.Specific.AccountDetail
{
    public class ImportFlowFinancialData1CIdentity : OperationIdentityBase<ImportFlowFinancialData1CIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportFlowFinancialData1CIdentity; }
        }

        public override string Description
        {
            get { return "Импорт оплат из 1С через шину"; }
        }
    }
}