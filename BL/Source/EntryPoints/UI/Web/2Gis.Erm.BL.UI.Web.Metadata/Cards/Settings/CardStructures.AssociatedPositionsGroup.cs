using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AssociatedPositionsGroup =
            CardMetadata.For<AssociatedPositionsGroup>()
                        .MainAttribute<AssociatedPositionsGroup, IAssociatedPositionsGroupViewModel>(x => x.Name)
                        .ConfigCommonCardToolbar()
                        .ConfigRelatedItems(UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config.Name.Static("AssociatedPosition")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelAssociatedPosition)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.AssociatedPosition)
                                                             .FilterToParent());
    }
}