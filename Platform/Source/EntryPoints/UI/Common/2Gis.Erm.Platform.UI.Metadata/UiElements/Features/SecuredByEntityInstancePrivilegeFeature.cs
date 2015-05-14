using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public sealed class SecuredByEntityInstancePrivilegeFeature : ISecuredElementFeature
    {
        public SecuredByEntityInstancePrivilegeFeature(EntityAccessTypes privilege, EntityName entity)
        {
            Entity = entity;
            Privilege = privilege;
        }

        public EntityAccessTypes Privilege { get; private set; }

        public EntityName Entity { get; private set; }
    }
}
