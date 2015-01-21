using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes
{
    [DataContract]
    public sealed class SetAsDefaultThemeIdentity : OperationIdentityBase<SetAsDefaultThemeIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return NuClear.Model.Common.Operations.Identity.OperationIdentityIds.SetAsDefaultThemeIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сделать основной тематикой";
            }
        }
    }
}