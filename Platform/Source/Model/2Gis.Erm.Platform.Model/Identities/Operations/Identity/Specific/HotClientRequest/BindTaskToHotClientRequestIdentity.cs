using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class BindTaskToHotClientRequestIdentity : OperationIdentityBase<BindTaskToHotClientRequestIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BindTaskToHotClientRequestIdentity; }
        }

        public override string Description
        {
            get { return "Связывание заявки на горячего клиента с id соответствующего задания в MSCRM"; }
        }
    }
}