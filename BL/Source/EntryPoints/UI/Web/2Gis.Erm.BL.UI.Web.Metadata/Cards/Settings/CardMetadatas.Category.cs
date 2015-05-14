using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Category =
            CardMetadata.For<Category>()
                        .WithEntityIcon()
                        .Actions
                        .Attach(ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.Category())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Category(), Icons.Icons.Entity.Small(EntityType.Instance.Category()), () => ErmConfigLocalization.CrdRelCategory),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityType.Instance.CategoryOrganizationUnit(), () => ErmConfigLocalization.CrdRelCategoryOU)
                                                      .AppendapleEntity<OrganizationUnit>());
    }
}