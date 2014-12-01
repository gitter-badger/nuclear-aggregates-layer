using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata RolePrivilege =
            CardMetadata.For<RolePrivilege>()
                        .MainAttribute(x => x.Id)
                        .ConfigCommonCardToolbar();
    }
}