using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AfterSaleServices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.AfterSalesService
{
    public sealed class CreateAfterSaleServiceActivitiesHandler : RequestHandler<CreateAfterSaleServiceActivitiesRequest, CreateAfterSaleServiceActivitiesResponse>
    {
        private readonly IFinder _finder;
        private readonly IDealCreateAfterSaleServiceAggregateService _dealCreateAfterSaleServiceAggregateService;

        public CreateAfterSaleServiceActivitiesHandler(IFinder finder, IDealCreateAfterSaleServiceAggregateService dealCreateAfterSaleServiceAggregateService)
        {
            _finder = finder;
            _dealCreateAfterSaleServiceAggregateService = dealCreateAfterSaleServiceAggregateService;
        }

        internal List<AfterSaleServiceActivity> CreateAfterSaleServiceActivities(IEnumerable<DealDto> dealsWithOrders,
                                                                                 Dictionary<long, List<AfterSaleServiceActivity>> existingAfterSaleActivities)
        {
            var newAfterSaleActivities = new List<AfterSaleServiceActivity>();

            foreach (var deal in dealsWithOrders)
            {
                List<AfterSaleServiceActivity> currentDealActivities;
                bool currentDealActivitiesAdded = !existingAfterSaleActivities.TryGetValue(deal.DealId, out currentDealActivities);
                if (currentDealActivitiesAdded)
                {
                    currentDealActivities = new List<AfterSaleServiceActivity>();
                }

                foreach (var orderGroup in deal.Orders.GroupBy(x => x.ReleaseCountFact))
                {
                    var releasesNumber = orderGroup.Key;
                    var absoluteBeginReleaseNumber = orderGroup.First().BeginDistributionDate.ToAbsoluteReleaseNumber();

                    var plan = BuildAfterSaleServicePlan(absoluteBeginReleaseNumber, releasesNumber);

                    foreach (var planItem in plan)
                    {
                        // Проверяем, существует в базе или только что заведенное, но не сохраненное ППС-действие.
                        bool activityExists = ExistsSimilarActivity(currentDealActivities, planItem);
                        if (!activityExists)
                        {
                            var newActivity = new AfterSaleServiceActivity
                            {
                                AbsoluteMonthNumber = planItem.AbsoluteReleaseNumber,
                                AfterSaleServiceType = (byte)planItem.Type,
                                DealId = deal.DealId
                            };

                            newAfterSaleActivities.Add(newActivity);
                            currentDealActivities.Add(newActivity);
                        }
                    }
                }

                if (currentDealActivitiesAdded)
                {
                    existingAfterSaleActivities[deal.DealId] = currentDealActivities;
                }
            }

            return newAfterSaleActivities;
        }

        protected override CreateAfterSaleServiceActivitiesResponse Handle(CreateAfterSaleServiceActivitiesRequest request)
        {
            // TODO : если Security.UserOrganizationUnit будет перенесена в тот же .edmx файл, то у 
            // следующих 2 подзапросов нужно/можно будет убрать ToArray().

            // Пользователи, принадлежащие указанному городу.
            var usersSubquery = _finder.Find<UserOrganizationUnit>(user => user.OrganizationUnitId == request.OrganizationUnitId).Select(user => user.UserId).ToArray();

            // Пользователи из управляющей компании.
            var headDeparmentUsersSubquery = _finder.Find<User>(user => user.Department.ParentId == null).Select(user => user.Id).ToArray();

            /*
            Поиск активных сделок города с заказами, по которому происходит формирование ППС:
            Находим все сделки привязанные к куратору города указанного в форме формирования ППС (Deals.OwnerCode -> UserOrganizationUnits.OrganizationUnitId), сделки активные, не удалённые;
            Отбрасываем сделки привязанные к клиенту "ДубльГИС-Региональная сеть";
            Выбираем все сделки к которым привязан хоть 1 заказ удовлетворяющий условиям (Orders.DealId):
            активные, не удалённые, 
            в статусе "Одобрено", "На расторжении";
            заказы с типом: "Продажи", "Рекламный бартер", "Товарный бартер", "Бартер на услуги", "Социальная реклама". Заказы с типом: "Самореклама", "ТРУ" не берём;
            по которым есть активная блокировка, на указанный в форме создания ППС месяц, и эта блокировка единственная (Locks.PeriodStartDate);
            */

            var dealsWithOrders = _finder.Find<Deal>(x => x.IsActive && !x.IsDeleted &&
                                                          usersSubquery.Contains(x.OwnerCode) &&
                                                          !headDeparmentUsersSubquery.Contains(x.Client.OwnerCode))
                                         .Select(d => new DealDto
                                             {
                                                 DealId = d.Id,
                                                 DealReplicationCode = d.ReplicationCode,
                                                 Orders = d.Orders
                                                           .Where(o => o.IsActive && !o.IsDeleted &&
                                                                       (o.WorkflowStepId == OrderState.Approved ||
                                                                        o.WorkflowStepId == OrderState.OnTermination) &&
                                                                       o.OrderType != OrderType.SelfAds &&
                                                                       o.OrderType != OrderType.None &&
                                                                       o.OrderType != OrderType.ProductAdsService &&
                                                                       o.Locks.Count(l => l.IsActive && !l.IsDeleted && l.PeriodStartDate == request.Month.Start) == 1 &&
                                                                       o.Locks.Count(l => !l.IsDeleted) == 1)
                                                           .Select(x => new OrderDto
                                                               {
                                                                   ReleaseCountFact = x.ReleaseCountFact,
                                                                   BeginDistributionDate = x.BeginDistributionDate
                                                               })
                                             })
                                         .Where(x => x.Orders.Any())
                                         .ToArray();

            if (dealsWithOrders.Length == 0)
            {
                throw new NotificationException(BLResources.CreateAfterSaleServiceActivitiesEmpty);
            }

            var dealIds = dealsWithOrders.Select(d => d.DealId).ToArray();
            int dealsCount = dealIds.Length;

            // Уже существующие ППС по сделкам.
            var existingAfterSaleActivities = _finder.Find<AfterSaleServiceActivity>(a => dealIds.Contains(a.DealId))
                                                     .GroupBy(d => d.DealId)
                                                     .ToDictionary(x => x.Key, x => x.ToList());

            var newAfterSaleActivities = CreateAfterSaleServiceActivities(dealsWithOrders, existingAfterSaleActivities);
            if (newAfterSaleActivities.Count > 0)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    foreach (var newActivity in newAfterSaleActivities)
                    {
                        _dealCreateAfterSaleServiceAggregateService.Create(newActivity);
                    }

                    transaction.Complete();
                }
            }

            return new CreateAfterSaleServiceActivitiesResponse(newAfterSaleActivities, dealsCount);
        }

        private static bool ExistsSimilarActivity(IEnumerable<AfterSaleServiceActivity> currentDealActivities, PlanItem planItem)
        {
            if (planItem.Type == AfterSaleServiceType.ASS2 || planItem.Type == AfterSaleServiceType.ASS3)
            {
                // ППС2 и ППС3 кагбэ взаимозаменяемы.
                return currentDealActivities.Any(x => x.AbsoluteMonthNumber == planItem.AbsoluteReleaseNumber &&
                    (x.AfterSaleServiceType == (byte)AfterSaleServiceType.ASS2 || x.AfterSaleServiceType == (byte)AfterSaleServiceType.ASS3));
            }

            return currentDealActivities.Any(x => x.AbsoluteMonthNumber == planItem.AbsoluteReleaseNumber && x.AfterSaleServiceType == (byte)planItem.Type);
        }

        private static IEnumerable<PlanItem> BuildAfterSaleServicePlan(int beginReleaseNumber, int releasesCount)
        {
            if (releasesCount < 1)
            {
                throw new ArgumentOutOfRangeException("releasesCount");
            }

            if (releasesCount > 1)
            {
                yield return new PlanItem(beginReleaseNumber, AfterSaleServiceType.ASS1);
            }

            var lastReleaseNumber = beginReleaseNumber + releasesCount - 1;
            for (var counter = beginReleaseNumber + 1; counter < lastReleaseNumber; counter++)
            {
                if (counter == beginReleaseNumber + 1)
                {
                    yield return new PlanItem(counter, AfterSaleServiceType.ASS2);
                }
                else
                {
                    yield return new PlanItem(counter, AfterSaleServiceType.ASS3);
                }
            }

            yield return new PlanItem(lastReleaseNumber, AfterSaleServiceType.ASS4);
        }

        internal class DealDto
        {
            public long DealId { get; set; }
            public Guid DealReplicationCode { get; set; }
            public IEnumerable<OrderDto> Orders { get; set; }
        }

        internal class OrderDto
        {
            public short ReleaseCountFact { get; set; }
            public DateTime BeginDistributionDate { get; set; }
        }

        private class PlanItem
        {
            public PlanItem(int absoluteReleaseNumber, AfterSaleServiceType type)
            {
                AbsoluteReleaseNumber = absoluteReleaseNumber;
                Type = type;
            }

            public int AbsoluteReleaseNumber { get; private set; }
            public AfterSaleServiceType Type { get; private set; }
        }
    }
}
