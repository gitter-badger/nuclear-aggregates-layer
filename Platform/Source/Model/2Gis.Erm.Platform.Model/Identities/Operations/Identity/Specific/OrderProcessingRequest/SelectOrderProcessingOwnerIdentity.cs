using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

using OperationIdentityIds = DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.OperationIdentityIds;

namespace DoubleGis.Erm.Model.Metadata.Operations.Identity.Specific.OrderProcessingRequest
{
    [DataContract]
    public sealed class SelectOrderProcessingOwnerIdentity : OperationIdentityBase<SelectOrderProcessingOwnerIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SelectOrderProcessingOwnerIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Выбор куратора для результата обработки заявки по заказу";
            }
        }
    }
}