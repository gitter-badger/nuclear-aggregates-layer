using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    /// <summary>
    /// Список сущностей системы с разбиением по агрегатам
    /// </summary>
    public static class AggregatesList
    {
        /// <summary>
        /// Список агрегатов системы. 
        /// </summary>
        public static readonly IDictionary<IEntityType, AggregateDescriptor> Aggregates = new Dictionary<IEntityType, AggregateDescriptor>
            {
                // COMMENT {s.pomadin, 31.07.2014}: опять же нужно определиться какое место в domain model знаимают действия, один это агрегат или несколько и т.п., т.к. ответы на эти вопросы влияют на то как функционал действий будет разложен по системе
                { ActivityAggregate.Root, new AggregateDescriptor(ActivityAggregate.Root, ActivityAggregate.Entities) },
                { AppointmentAggregate.Root, new AggregateDescriptor(AppointmentAggregate.Root, AppointmentAggregate.Entities) },
                { LetterAggregate.Root, new AggregateDescriptor(LetterAggregate.Root, LetterAggregate.Entities) },
                { PhonecallAggregate.Root, new AggregateDescriptor(PhonecallAggregate.Root, PhonecallAggregate.Entities) },
                { TaskAggregate.Root, new AggregateDescriptor(TaskAggregate.Root, TaskAggregate.Entities) },        
       
                { AccountAggregate.Root, new AggregateDescriptor(AccountAggregate.Root, AccountAggregate.Entities) },
                { AdvertisementAggregate.Root, new AggregateDescriptor(AdvertisementAggregate.Root, AdvertisementAggregate.Entities) },
                { BranchOfficeAggregate.Root, new AggregateDescriptor(BranchOfficeAggregate.Root, BranchOfficeAggregate.Entities) },
                { ChargeAggregate.Root, new AggregateDescriptor(ChargeAggregate.Root, ChargeAggregate.Entities) },
                /* агрегат billing - бизнес смысл - учет откруток и т.п., пока ничего кроме charge не нашлось*/
                { ClientAggregate.Root, new AggregateDescriptor(ClientAggregate.Root, ClientAggregate.Entities) },
                { FirmAggregate.Root, new AggregateDescriptor(FirmAggregate.Root, FirmAggregate.Entities) },
                { LegalPersonAggregate.Root, new AggregateDescriptor(LegalPersonAggregate.Root, LegalPersonAggregate.Entities) },
                { LocalMessageAggregate.Root, new AggregateDescriptor(LocalMessageAggregate.Root, LocalMessageAggregate.Entities) },
                /* скорее агрегат, чем нет*/{ DealAggregate.Root, new AggregateDescriptor(DealAggregate.Root, DealAggregate.Entities) },
                /*не агрегат*/{ BargainAggregate.Root, new AggregateDescriptor(BargainAggregate.Root, BargainAggregate.Entities) },
                { OrderAggregate.Root, new AggregateDescriptor(OrderAggregate.Root, OrderAggregate.Entities) },
                { PositionAggregate.Root, new AggregateDescriptor(PositionAggregate.Root, PositionAggregate.Entities) },
                { PriceAggregate.Root, new AggregateDescriptor(PriceAggregate.Root, PriceAggregate.Entities) },
                /*не агрегат?*/{ ReleaseAggregate.Root, new AggregateDescriptor(ReleaseAggregate.Root, ReleaseAggregate.Entities) },
                /*не агрегат*/{ RoleAggregate.Root, new AggregateDescriptor(RoleAggregate.Root, RoleAggregate.Entities) },
                /*не агрегат?*/{ ThemeAggregate.Root, new AggregateDescriptor(ThemeAggregate.Root, ThemeAggregate.Entities) },
                { UserAggregate.Root, new AggregateDescriptor(UserAggregate.Root, UserAggregate.Entities) },
                /*не агрегат?*/{ OrganizationUnitAggregate.Root, new AggregateDescriptor(OrganizationUnitAggregate.Root, OrganizationUnitAggregate.Entities) }
            };

        /// <summary>
        /// Найти агрегат, элементом которого является указанная aggregateEntity
        /// Если такой агрегат не один (пока в рамках исключения разрешены амбивалентные сущности) - то будет брошен exception
        /// </summary>
        public static IEntityType ToSingleRoot(this IEntityType aggregateEntity)
        {
            return aggregateEntity.ToAggregates().SingleOrDefault();
        }

        /// <summary>
        /// Найти агрегаты, элементом которых является указанная aggregateEntity
        /// В большинстве случаев таких агрегатов будет не больше одного, однако, пока в рамках исключения разрешены амбивалентные сущности - может быть и больше одного
        /// </summary>
        public static IEntityType[] ToAggregates(this IEntityType aggregateEntity)
        {
            return Aggregates
                .Where(d => d.Value.AggregateEntities.Contains(aggregateEntity))
                .Select(d => d.Value.AggregateRoot)
                .ToArray();
        }
    }
}
