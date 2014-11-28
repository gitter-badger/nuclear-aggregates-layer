using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata CategoryGroup =
            CardMetadata.Config
                        .For<CategoryGroup>()
                        .MainAttribute<CategoryGroup, ICategoryGroupViewModel>(x => x.CategoryGroupName)                
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<CategoryGroup>());
    }
}