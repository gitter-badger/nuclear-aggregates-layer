using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AfterSaleService
{
    [DataContract]
    public sealed class UpdateAfterSaleServiceIdentity : OperationIdentityBase<UpdateAfterSaleServiceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.UpdateAfterSaleServiceIdentity; }
        }

        public override string Description
        {
            get { return "Synch after sale service state for period with external system (MSCRM)"; }
        }
    }
}
