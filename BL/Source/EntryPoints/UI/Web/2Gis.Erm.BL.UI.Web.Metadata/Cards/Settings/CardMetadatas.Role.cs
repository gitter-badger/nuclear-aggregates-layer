using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Role =
            CardMetadata.For<Role>()
                        .MainAttribute<Role, IRoleViewModel>(x => x.Name)
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_UserGroup.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("RoleEntityPrivilege")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelRoleEntityPrivilege)
                                                             .FilterToParent()
                                                             .Handler.Request("/Edit/EntityPrivileges")
                                                             .LockOnNew(),
                                            UIElementMetadata.Config
                                                             .Name.Static("RoleFunctionalPrivilege")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelRoleFunctionalPrivilege)
                                                             .FilterToParent()
                                                             .Handler.Request("/Edit/FunctionalPrivileges")
                                                             .LockOnNew());
    }
}