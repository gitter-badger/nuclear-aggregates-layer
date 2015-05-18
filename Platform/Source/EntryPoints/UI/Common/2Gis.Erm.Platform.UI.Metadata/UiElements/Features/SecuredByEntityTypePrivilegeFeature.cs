using DoubleGis.Erm.Platform.API.Security.EntityAccess;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public sealed class SecuredByEntityTypePrivilegeFeature : ISecuredElementFeature
    {
        public SecuredByEntityTypePrivilegeFeature(EntityAccessTypes privilege, IEntityType entity)
        {
            Entity = entity;
            Privilege = privilege;
        }

        public EntityAccessTypes Privilege { get; private set; }

        public IEntityType Entity { get; private set; }
    }
}
