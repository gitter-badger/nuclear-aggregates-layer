namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class GetFirmWorkStatusIdentity : OperationIdentityBase<GetFirmWorkStatusIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetFirmWorkStatusIdentity; }
        }

        public override string Description
        {
            get { return "Получение данных по фирме для IR"; }
        }
    }
}