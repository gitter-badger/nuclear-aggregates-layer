using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Entities;

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
        public static readonly IDictionary<EntityName, AggregateDescriptor> Aggregates = 
            new[]
            {
                // COMMENT {s.pomadin, 31.07.2014}: опять же нужно определиться какое место в domain model знаимают действия, один это агрегат или несколько и т.п., т.к. ответы на эти вопросы влияют на то как функционал действий будет разложен по системе
                ActivityAggregate.Activity.ToDescriptor(),
                AppointmentAggregate.Appointment.ToDescriptor(),
                LetterAggregate.Letter.ToDescriptor(),
                PhonecallAggregate.Phonecall.ToDescriptor(),
                TaskAggregate.Task.ToDescriptor(),

                AccountAggregate.Account.ToDescriptor(), 
                AdvertisementAggregate.Advertisement.ToDescriptor(),
                BranchOfficeAggregate.BranchOffice.ToDescriptor(),
                ChargeAggregate.Charge.ToDescriptor(), /* агрегат billing - бизнес смысл - учет откруток и т.п., пока ничего кроме charge не нашлось*/
                ClientAggregate.Client.ToDescriptor(),
                FirmAggregate.Firm.ToDescriptor(),
                LegalPersonAggregate.LegalPerson.ToDescriptor(),
                LocalMessageAggregate.LocalMessage.ToDescriptor(),
                /* скорее агрегат, чем нет*/DealAggregate.Deal.ToDescriptor(),
                /*не агрегат*/BargainAggregate.Bargain.ToDescriptor(),
                OrderAggregate.Order.ToDescriptor(),
                PositionAggregate.Position.ToDescriptor(),
                PriceAggregate.Price.ToDescriptor(),
                /*не агрегат?*/ReleaseAggregate.ReleaseInfo.ToDescriptor(),
                /*не агрегат*/RoleAggregate.Role.ToDescriptor(),
                /*не агрегат?*/ThemeAggregate.Theme.ToDescriptor(),
                UserAggregate.User.ToDescriptor(),
                /*не агрегат?*/OrganizationUnitAggregate.OrganizationUnit.ToDescriptor()
            }
            .ToDictionary(descriptor => descriptor.AggregateRoot, descriptor => descriptor);

        /// <summary>
        /// Найти агрегат, элементом которого является указанная aggregateEntity
        /// Если такой агрегат не один (пока в рамках исключения разрешены амбивалентные сущности) - то будет брошен exception
        /// </summary>
        public static EntityName? ToSingleRoot(this EntityName aggregateEntity)
        {
            return aggregateEntity.ToAggregates().SingleOrDefault();
        }

        /// <summary>
        /// Найти агрегаты, элементом которых является указанная aggregateEntity
        /// В большинстве случаев таких агрегатов будет не больше одного, однако, пока в рамках исключения разрешены амбивалентные сущности - может быть и больше одного
        /// </summary>
        public static EntityName[] ToAggregates(this EntityName aggregateEntity)
        {
            return Aggregates
                .Where(d => d.Value.AggregateEntities.Contains(aggregateEntity))
                .Select(d => d.Value.AggregateRoot)
                .ToArray();
        }
    }
}
