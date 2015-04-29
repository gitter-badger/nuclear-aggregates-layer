using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntityNameUtils
    {
        /// <summary>
        /// Список значений EntityName, являющихся виртуальными, т.е. существующими только в виде UI-форм
        /// </summary>
        // TODO {all, 02.04.2015}: в случае появления времени или 3-ей записи необходимо выделить хранилище настроек карточек виртуальных сущностей
        public static readonly IEntityType[] VirtualEntityNames = { EntityType.Instance.CategoryGroupMembership(), EntityType.Instance.PositionSortingOrder() };

        /// <summary>
        /// Список значений EntityName, являющихся расширением для какой-либо инсталляции, данные fake сущности не являются элементами доменной модели ERM, 
        /// а являются просто удобными контейнерами для работы с доп.аттрибутами (своего рода attached properties) доменных сущностей, которые появляются у сущности только в некоторых businessmodel 
        /// </summary>
        public static readonly IEntityType[] EntityParts =
            {
                EntityType.Instance.ChileLegalPersonPart(),
                EntityType.Instance.UkraineLegalPersonPart(),
                EntityType.Instance.ChileBranchOfficeOrganizationUnitPart(),
                EntityType.Instance.UkraineBranchOfficePart(),
                EntityType.Instance.ChileLegalPersonProfilePart(),
                EntityType.Instance.UkraineLegalPersonProfilePart(),
                EntityType.Instance.EmiratesBranchOfficeOrganizationUnitPart(),
                EntityType.Instance.EmiratesClientPart(),
                EntityType.Instance.EmiratesLegalPersonPart(),
                EntityType.Instance.EmiratesLegalPersonProfilePart(),
                EntityType.Instance.EmiratesFirmAddressPart(),
                EntityType.Instance.KazakhstanLegalPersonPart(),
                EntityType.Instance.KazakhstanLegalPersonProfilePart()
            };

        /// <summary>
        /// Список значений EntityName динамических сущностей
        /// </summary>
        public static readonly IEntityType[] DynamicEntities =
            {
                EntityType.Instance.Bank(),
                EntityType.Instance.Commune(),
            };

        public static readonly IEntityType[] MappingEntities =
            {
                EntityType.Instance.Appointment(),
                EntityType.Instance.AppointmentRegardingObject(),
                EntityType.Instance.AppointmentAttendee(),
                EntityType.Instance.AppointmentOrganizer(),
                EntityType.Instance.Phonecall(),
                EntityType.Instance.PhonecallRegardingObject(),
                EntityType.Instance.PhonecallRecipient(),
                EntityType.Instance.Task(),
                EntityType.Instance.TaskRegardingObject(),
                EntityType.Instance.Letter(),
                EntityType.Instance.LetterRegardingObject(),
                EntityType.Instance.LetterSender(),
                EntityType.Instance.LetterRecipient(),
            };

        public static readonly Type[] AllReplicated2MsCrmEntities =
            {
                typeof(OrganizationUnit),
                typeof(Currency),
                typeof(Category),
                typeof(Territory),
                typeof(Client),
                typeof(Firm),
                typeof(FirmAddress),
                typeof(Contact),
                typeof(Position),
                typeof(BranchOffice),
                typeof(BranchOfficeOrganizationUnit),
                typeof(LegalPerson),
                typeof(Account),
                typeof(OperationType),
                typeof(AccountDetail),
                typeof(Deal),
                typeof(Limit),
                typeof(Order),
                typeof(OrderPosition),
                typeof(Bargain),
                typeof(OrderProcessingRequest),
                typeof(User),
                typeof(UserTerritory),
                typeof(Firm),
                typeof(FirmAddress),
                typeof(Territory),
                typeof(Appointment),
                typeof(Letter),
                typeof(Phonecall),
                typeof(Task)
            };

        /// <summary>
        /// Список типов ERM существующих только на уровне persistance, используемых только в DAL и не используемых в более высокоуровневых слоях, чем агрегирующие репозитории 
        /// </summary>
        public static readonly HashSet<Type> PersistenceOnlyEntities = new HashSet<Type>
            {
                typeof(FunctionalPrivilegeDepth),
                typeof(OrganizationUnitDto),
                typeof(Privilege),
                typeof(TerritoryDto),
                typeof(UsersDescendant),
                typeof(BusinessOperationService),
                typeof(SecurityAccelerator),

                typeof(AppointmentBase),
                typeof(AppointmentReference),
                typeof(PhonecallBase),
                typeof(PhonecallReference),
                typeof(TaskBase),
                typeof(TaskReference),
                typeof(LetterBase),
                typeof(LetterReference),
            };

        /// <summary>
        /// Список сущностей ERM, обычно справочных, которые должны иметь строго одну и ту же identity в разных инсталляциях системы (например, на Кипре и в России)
        /// Т.к. пока автоматический sharing (репликация и т.п.) экземпляров таких сущностей не реализован, то наличие сущности в этом списке означает, 
        /// что для неё не всегда нужно генерировать ID, иногда ID вводится явно в ручном режиме администраторами системы
        /// </summary>
        // TODO {all, 29.07.2013}: Перевести shared entities на генерацию Id в master инсталяции Erm, с отключением возможности ручного задания ID администраторами системы
        public static readonly HashSet<Type> InstanceSharedEntities = new HashSet<Type>
            {
                typeof(AdvertisementElementTemplate),
                typeof(AdvertisementTemplate),
                typeof(BargainType),
                typeof(BranchOfficeOrganizationUnit),
                typeof(BranchOffice),
                typeof(Country),
                typeof(ContributionType),
                typeof(Currency),
                typeof(OperationType),
                typeof(OrganizationUnit),
                typeof(Erm.Platform),
                typeof(PositionCategory),
                typeof(CategoryGroup),
                typeof(Territory),
                typeof(Department),
                typeof(Privilege),
                typeof(Role),
                typeof(User),
                typeof(MessageType),
                typeof(Security.TimeZone),
                typeof(Position),
                typeof(Theme),
                typeof(UserProfile),
                typeof(Project)
            };

        public static bool IsPart(this IEntityType entityName)
        {
            return EntityParts.Contains(entityName);
        }

        public static bool IsDynamic(this IEntityType entityName)
        {
            return DynamicEntities.Contains(entityName);
        }

        public static bool HasMapping(this IEntityType entityName)
        {
            return MappingEntities.Contains(entityName);
        }

        public static bool IsInstanceShared(this Type checkingType)
        {
            return InstanceSharedEntities.Contains(checkingType);
        }

        public static bool IsBaseEntity(this Type entityType)
        {
            return typeof(IBaseEntity).IsAssignableFrom(entityType);
        }

        public static bool IsEntity(this Type entityType)
        {
            return typeof(IEntity).IsAssignableFrom(entityType);
        }

        public static bool IsSecurableAccessRequired(this IEntityType entityName)
        {
            return entityName.AsEntityType().IsSecurableAccessRequired();
        }

        public static bool IsSecurableAccessRequired(this Type entityType)
        {
            if (!entityType.IsEntity())
            {
                throw new InvalidOperationException("Specified type " + entityType + " is not domain model entity");
            }

            return typeof(ICuratedEntity).IsAssignableFrom(entityType);
        }

        public static bool IsOwnerable(this IEntityType entityName)
        {
            return typeof(ICuratedEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsDeactivatable(this IEntityType entityName)
        {
            return typeof(IDeactivatableEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsDeletable(this IEntityType entityName)
        {
            return typeof(IDeletableEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsFileEntity(this IEntityType entityName)
        {
            Type entityType = entityName.AsEntityType();
            return typeof(IEntityFile).IsAssignableFrom(entityType) || typeof(IEntityFileOptional).IsAssignableFrom(entityType);
        }
    }
}
