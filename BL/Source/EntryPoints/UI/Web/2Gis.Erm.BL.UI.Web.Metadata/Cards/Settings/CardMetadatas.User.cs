using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata User =
            CardMetadata.For<User>()
                        .Icon.Path(Icons.Icons.Entity.User)
                        .Actions
                        .Attach(ToolbarElements.Create<User>(),
                                ToolbarElements.Update<User>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<User>(),
                                ToolbarElements.UpdateAndClose<User>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<User>(),
                                ToolbarElements.Additional(ToolbarElements.Users.Profile()),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.UserSmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("UserRole")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelUserRole)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.UserRole)
                                                           .FilterToParent()
                                                           .AppendapleEntity<Role>(),
                                          UIElementMetadata.Config
                                                           .Name.Static("UserTerritory")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelUserTerritory)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.UserTerritory)
                                                           .FilterToParent()
                                                           .AppendapleEntity<Territory>(),
                                          UIElementMetadata.Config
                                                           .Name.Static("UserOrganizationUnit")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelUserOrganizationUnit)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.UserOrganizationUnit)
                                                           .FilterToParent()
                                                           .AppendapleEntity<OrganizationUnit>());
    }
}