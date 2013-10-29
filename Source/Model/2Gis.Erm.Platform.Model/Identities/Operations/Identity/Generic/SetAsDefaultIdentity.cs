using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class SetAsDefaultIdentity : OperationIdentityBase<SetAsDefaultIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SetAsDefaultIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сделать основной сущнотью";
            }
        }
    }
}