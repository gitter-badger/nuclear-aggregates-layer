using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class ListNonGenericIdentity : OperationIdentityBase<ListNonGenericIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ListNonGenericIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "ListNonGenericIdentity";
            }
        }
    }
}
