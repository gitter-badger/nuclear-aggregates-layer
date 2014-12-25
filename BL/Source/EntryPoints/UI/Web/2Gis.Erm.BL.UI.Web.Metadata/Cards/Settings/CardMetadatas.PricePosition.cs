using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata PricePosition =
            CardMetadata.For<PricePosition>()
                        .Icon.Path(Icons.Icons.Entity.PricePosition)
                        .Actions
                        .Attach(ToolbarElements.Create<PricePosition>(),
                                ToolbarElements.Update<PricePosition>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<PricePosition>(),
                                ToolbarElements.UpdateAndClose<PricePosition>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<PricePosition>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.PricePositions.Copy()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.PricePositionSmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("AssociatedPositionsGroup")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelAssociatedPositionsGroup)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.AssociatedPositionsGroup)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("DeniedPosition")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelDeniedPosition)
                                                           .ExtendedInfo(new TemplateDescriptor(
                                                                             new StaticStringResourceDescriptor("PositionId={0}&&PriceId={1}"),
                                                                             new PropertyDescriptor<IPricePositionViewModel>(x => x.Position.Key),
                                                                             new PropertyDescriptor<IPricePositionViewModel>(x => x.Price.Key)))
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.DeniedPosition));
    }
}