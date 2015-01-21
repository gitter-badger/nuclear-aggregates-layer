using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail
{
    public class NotifyAboutAccountDetailModificationIdentity : OperationIdentityBase<NotifyAboutAccountDetailModificationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.NotifyAboutAccountDetailModificationIdentity; }
        }

        public override string Description
        {
            get { return "Уведомление менеджера о регистрации/изменении операции по лицевому счету"; }
        }
    }
}