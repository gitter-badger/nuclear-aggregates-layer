using System;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    [Obsolete("Данный подход не удовлетворяет бизнес-логике валидации прав. Нужно использовать SecuredByEntityTypePrivilegeFeature если нужна проверка прав в рамках сущности и SecuredByEntityInstancePrivilegeFeature если нужна проверка прав в рамках экземпляра сущности.")]
    public sealed class SecuredByEntityPrivelegeFeature : ISecuredElementFeature
    {
        public SecuredByEntityPrivelegeFeature(EntityAccessTypes privilege, EntityName entity)
        {
            Entity = entity;
            Privilege = privilege;
        }

        public EntityAccessTypes Privilege { get; private set; }

        public EntityName Entity { get; private set; }
    }
}