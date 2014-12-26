using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static class Role
        {
            public static UIElementMetadataBuilder EntityPrivilege()
            {
                return UIElementMetadata.Config
                                        .Name.Static("RoleEntityPrivilege")
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelRoleEntityPrivilege)
                                        .FilterToParent()
                                        .Handler.Request("/Edit/EntityPrivileges")
                                        .LockOnNew();
            }

            public static UIElementMetadataBuilder FunctionalPrivilege()
            {
                return UIElementMetadata.Config
                                        .Name.Static("RoleFunctionalPrivilege")
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelRoleFunctionalPrivilege)
                                        .FilterToParent()
                                        .Handler.Request("/Edit/FunctionalPrivileges")
                                        .LockOnNew();
            }
        }
    }
}
