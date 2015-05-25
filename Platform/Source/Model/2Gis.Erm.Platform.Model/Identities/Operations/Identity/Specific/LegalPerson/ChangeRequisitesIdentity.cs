using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson
{
    public sealed class ChangeRequisitesIdentity : OperationIdentityBase<ChangeRequisitesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeLegalPersonRequisitesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Изменить реквизиты юрлица";
            }
        }
    }
}