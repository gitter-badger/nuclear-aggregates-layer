using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata User =
            CardMetadata.For<User>()
                        .WithEntityIcon()
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
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.User())),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityType.Instance.UserRole(), () => ErmConfigLocalization.CrdRelUserRole)
                                                      .AppendapleEntity<Role>(),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityType.Instance.UserTerritory(), () => ErmConfigLocalization.CrdRelUserTerritory)
                                                      .AppendapleEntity<Territory>(),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityType.Instance.UserOrganizationUnit(), () => ErmConfigLocalization.CrdRelUserOrganizationUnit)
                                                      .AppendapleEntity<OrganizationUnit>());
    }
}