using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Category =
            CardMetadata.For<Category>()
                        .Icon.Path(Icons.Icons.Entity.Category)
                        .Actions
                        .Attach(ToolbarElements.Close())
                        .WithRelatedItems(
                                          UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.CategorySmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("Category")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelCategory)
                                                           .LockOnNew()
                                                           .Icon.Path(Icons.Icons.Entity.CategorySmall)
                                                           .Handler.ShowGridByConvention(EntityName.Category)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("CategoryOrganizationUnit")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelCategoryOU)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.CategoryOrganizationUnit)
                                                           .FilterToParent()
                                                           .AppendapleEntity<OrganizationUnit>());
    }
}