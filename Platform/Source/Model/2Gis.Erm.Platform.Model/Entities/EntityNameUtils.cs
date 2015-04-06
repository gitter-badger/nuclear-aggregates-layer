using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static partial class EntityNameUtils
    {
        /// <summary>
        /// Список значений EntityName, являющихся composed - т.е. комбинирующими
        /// </summary>
        public static readonly EntityName[] ComposedEntityNames = { EntityName.None, EntityName.All };
        
        /// <summary>
        /// Список значений EntityName, являющихся виртуальными, т.е. существующими только в виде UI-форм
        /// </summary>
        public static readonly EntityName[] VirtualEntityNames = { EntityName.CategoryGroupMembership, EntityName.PositionSortingOrder };

        /// <summary>
        /// Список значений EntityName, являющихся расширением для какой-либо инсталляции, данные fake сущности не являются элементами доменной модели ERM, 
        /// а являются просто удобными контейнерами для работы с доп.аттрибутами (своего рода attached properties) доменных сущностей, которые появляются у сущности только в некоторых businessmodel 
        /// </summary>
        public static readonly EntityName[] EntityParts =
            {
                EntityName.ChileLegalPersonPart,
                EntityName.UkraineLegalPersonPart,
                EntityName.ChileBranchOfficeOrganizationUnitPart,
                EntityName.UkraineBranchOfficePart,
                EntityName.ChileLegalPersonProfilePart,
                EntityName.UkraineLegalPersonProfilePart,
                EntityName.EmiratesBranchOfficeOrganizationUnitPart,
                EntityName.EmiratesClientPart,
                EntityName.EmiratesLegalPersonPart,
                EntityName.EmiratesLegalPersonProfilePart,
                EntityName.EmiratesFirmAddressPart,
                EntityName.KazakhstanLegalPersonPart,
                EntityName.KazakhstanLegalPersonProfilePart
            };

        /// <summary>
        /// Список значений EntityName динамических сущностей
        /// </summary>
        public static readonly EntityName[] DynamicEntities =
            {
                EntityName.Bank,
                EntityName.Commune,
            };

		public static readonly EntityName[] MappingEntities =
            {
                EntityName.Appointment,
                EntityName.AppointmentRegardingObject,
				EntityName.AppointmentAttendee,
                EntityName.AppointmentOrganizer,
                EntityName.Phonecall,
                EntityName.PhonecallRegardingObject,
				EntityName.PhonecallRecipient,
                EntityName.Task,
                EntityName.TaskRegardingObject,
                EntityName.Letter,
                EntityName.LetterRegardingObject,
                EntityName.LetterSender,
                EntityName.LetterRecipient,
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

        /// <summary>
        /// Разложить composed значение на составляющие, если на вход передано не composed (элементарное) значение EntityName - возвращается оно без изменений
        /// </summary>
        public static EntityName[] GetDecomposed(this EntityName entityName)
        {
            if (entityName == EntityName.None)
            {
                return new EntityName[0];
            }

            if (entityName == EntityName.All)
            {
                var allValues = (EntityName[])Enum.GetValues(typeof(EntityName));
                return allValues
                    .Except(ComposedEntityNames)
                    //.Except(VirtualEntityNames)
                    .ToArray();
            }

            return new[] { entityName };
        }

        public static bool IsVirtual(this EntityName entityName)
        {
            return VirtualEntityNames.Contains(entityName);
        }

        public static bool IsPart(this EntityName entityName)
        {
            return EntityParts.Contains(entityName);
        }

        public static bool IsDynamic(this EntityName entityName)
        {
            return DynamicEntities.Contains(entityName);
        }

        public static bool HasMapping(this EntityName entityName)
        {
            return MappingEntities.Contains(entityName);
        }

        public static bool IsPersistenceOnly(this Type checkingType)
        {
            return PersistenceOnlyEntities.Contains(checkingType);
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

        public static bool IsSecurableAccessRequired(this EntityName entityName)
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

        public static bool IsOwnerable(this EntityName entityName)
        {
            return typeof(ICuratedEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsDeactivatable(this EntityName entityName)
        {
            return typeof(IDeactivatableEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsDeletable(this EntityName entityName)
        {
            return typeof(IDeletableEntity).IsAssignableFrom(entityName.AsEntityType());
        }

        public static bool IsFileEntity(this EntityName entityName)
        {
            Type entityType = entityName.AsEntityType();
            return typeof(IEntityFile).IsAssignableFrom(entityType) || typeof(IEntityFileOptional).IsAssignableFrom(entityType);
        }

        public static string EntitiesToString(this EntityName[] entityNames)
        {
            if (entityNames == null || entityNames.Length == 0)
            {
                return "Entities list is empty";
            }

            var sb = new StringBuilder();
            sb.Append(entityNames[0].ToString());
            for (int i = 1; i < entityNames.Length; i++)
            {
                sb.Append(";")
                  .Append(entityNames[i].ToString());
            }

            return sb.ToString();
        }
    }
}
