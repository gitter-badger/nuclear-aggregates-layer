using System;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    [Obsolete("Данный подход не удовлетворяет бизнес-логике валидации прав. Нужно использовать SecuredByEntityTypePrivilegeFeature если нужна проверка прав в рамках сущности и SecuredByEntityInstancePrivilegeFeature если нужна проверка прав в рамках экземпляра сущности.")]
    public sealed class SecuredByEntityPrivelegeFeature : ISecuredElementFeature
    {
        public SecuredByEntityPrivelegeFeature(EntityAccessTypes privilege, IEntityType entity)
        {
            Entity = entity;
            Privilege = privilege;
        }

        public EntityAccessTypes Privilege { get; private set; }

        public IEntityType Entity { get; private set; }
    }
}