using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Contact
{
    [DataContract]
    public sealed class RequestBirthdayCongratulationsIdentity : OperationIdentityBase<RequestBirthdayCongratulationsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RequestBirthdayCongratulationsIdentity; }
        }

        public override string Description
        {
            get { return "Запрос на поздравление именинников"; }
        }
    }
}
