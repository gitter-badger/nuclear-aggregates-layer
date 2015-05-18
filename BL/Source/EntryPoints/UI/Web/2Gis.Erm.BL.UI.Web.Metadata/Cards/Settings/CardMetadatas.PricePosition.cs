using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata PricePosition =
            CardMetadata.For<PricePosition>()
                        .InfoOn<IDeletableAspect>(x => x.IsDeleted, StringResourceDescriptor.Create(() => BLResources.CantEditDeactivatedPricePosition))
                        .WithEntityIcon()
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
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.PricePosition())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.AssociatedPositionsGroup(), () => ErmConfigLocalization.CrdRelAssociatedPositionsGroup),
                                          RelatedItems.RelatedItem.PricePosition.DeniedPositionsGrid());
    }
}