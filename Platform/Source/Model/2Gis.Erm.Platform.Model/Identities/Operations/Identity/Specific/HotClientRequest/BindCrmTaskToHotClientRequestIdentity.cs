namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class BindCrmTaskToHotClientRequestIdentity : OperationIdentityBase<BindCrmTaskToHotClientRequestIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BindCrmTaskToHotClientRequestIdentity; }
        }

        public override string Description
        {
            get { return "Связывание заявки на горячего клиента с id соответствующего задания в MSCRM"; }
        }
    }
}