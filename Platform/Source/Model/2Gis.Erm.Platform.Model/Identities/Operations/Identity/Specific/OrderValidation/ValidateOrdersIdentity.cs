using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation
{
    [DataContract]
    public class ValidateOrdersIdentity : OperationIdentityBase<ValidateOrdersIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ValidateOrdersIdentity; }
        }

        public override string Description
        {
            get { return "Validate order(s)"; }
        }
    }
}