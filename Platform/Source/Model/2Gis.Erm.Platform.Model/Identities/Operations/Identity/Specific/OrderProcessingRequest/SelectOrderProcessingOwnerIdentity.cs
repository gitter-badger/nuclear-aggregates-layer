using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

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