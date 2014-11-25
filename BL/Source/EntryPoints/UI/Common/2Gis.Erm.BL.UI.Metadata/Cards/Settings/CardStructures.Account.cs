using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .EntityLocalization(() => ErmConfigLocalization.EnAccounts)
                        .Icon.Path("en_ico_16_Account.gif")
                        .MainAttribute(x => x.Id);
    }
}