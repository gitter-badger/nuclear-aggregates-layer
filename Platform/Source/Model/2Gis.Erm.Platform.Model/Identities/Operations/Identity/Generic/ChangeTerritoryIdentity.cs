using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class ChangeTerritoryIdentity : OperationIdentityBase<ChangeTerritoryIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeTerritoryIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "ChangeTerritory";
            }
        }
    }
}