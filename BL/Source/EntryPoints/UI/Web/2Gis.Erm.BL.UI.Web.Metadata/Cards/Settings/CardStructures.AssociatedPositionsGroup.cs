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
        public static readonly CardMetadata AssociatedPositionsGroup =
            CardMetadata.Config
                        .For<AssociatedPositionsGroup>()
                        .MainAttribute<AssociatedPositionsGroup>(x => x.Id)
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<AssociatedPositionsGroup>())
                        .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config.Name.Static("AssociatedPosition")
                                                            .Title.Resource(() => ErmConfigLocalization.CrdRelAssociatedPosition)
                                                            .LockOnNew()
                                                            .Handler.ShowGridByConvention(EntityName.AssociatedPosition));
    }
}