namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail
{
    public class GetAccountDetailsForExportContentIdentity : OperationIdentityBase<GetAccountDetailsForExportContentIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetAccountDetailsForExportContentIdentity; }
        }

        public override string Description
        {
            get { return "Формирование данных для экспорта списаний"; }
        }
    }
}