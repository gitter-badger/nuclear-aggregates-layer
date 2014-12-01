using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Lock =
            CardMetadata.For<Lock>()
                        .MainAttribute(x => x.Id)
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("LockDetails")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLockDetails)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.LockDetail)
                                                             .FilterToParent());
    }
}