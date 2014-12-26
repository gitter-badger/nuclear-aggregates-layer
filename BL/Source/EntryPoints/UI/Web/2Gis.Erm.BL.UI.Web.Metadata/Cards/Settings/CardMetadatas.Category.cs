using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Category =
            CardMetadata.For<Category>()
                        .WithEntityIcon()
                        .Actions
                        .Attach(ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityName.Category)),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Category, Icons.Icons.Entity.Small(EntityName.Category), () => ErmConfigLocalization.CrdRelCategory),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityName.CategoryOrganizationUnit, () => ErmConfigLocalization.CrdRelCategoryOU)
                                                      .AppendapleEntity<OrganizationUnit>());
    }
}