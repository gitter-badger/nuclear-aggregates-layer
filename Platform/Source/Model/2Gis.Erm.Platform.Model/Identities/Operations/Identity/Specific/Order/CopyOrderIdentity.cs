using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    [DataContract]
    public sealed class CopyOrderIdentity : OperationIdentityBase<CopyOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CopyOrderIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Copy order";
            }
        }
    }
}
