using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка достаточности денежных средств на лицевом счете для размещения заказа
    /// </summary>
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class BalanceOrderValidationRule : OrderValidationRuleBase
    {
        private readonly IFinder _finder;
        private readonly IUseCaseTuner _useCaseTuner;

        public BalanceOrderValidationRule(IFinder finder, IUseCaseTuner useCaseTuner)
        {
            _finder = finder;
            _useCaseTuner = useCaseTuner;
        }

        // Выборка заказов происходит в 2 этапа. Этот метод вернет предикат для заказов, город ИСТОЧНИК которых равен городу, выбранному в форме
        // Далее при валидации накладывается еще один фильтр для заказов, по которым для города НАЗНАЧЕНИЯ не было финальной сборки за период, либо 
        //                                                                               город НАЗНАЧЕНИЯ не переведен на ERM
        // Итого для этой проверки получаем такой фильтр: 
        // Заказы, для которых город ИСТОЧНИК равен городу, выбранному в форме 
        // И ( [по городу НАЗНАЧЕНИЯ не было финальной сборки за период] ИЛИ [город НАЗНАЧЕНИЯ не переведен на ERM] )
        private Expression<Func<Order, bool>> GetOrdersOverridedPredicate(ValidateOrdersRequest request, OrderValidationPredicate p)
        {
            var finalReleaseInfos =
                _finder.Find(ReleaseSpecs.Releases.Find.FinalForPeriodWithStatuses(new TimePeriod(request.Period.Start, request.Period.End), ReleaseStatus.Success));

            var overridenPredicate = new OrderValidationPredicate(
                p.GeneralPart,
                o => (o.DestOrganizationUnitId == request.OrganizationUnitId
                        && !finalReleaseInfos.Any(x => x.OrganizationUnitId == o.SourceOrganizationUnitId))
                    || (o.SourceOrganizationUnitId == request.OrganizationUnitId
                        && (!finalReleaseInfos.Any(x => x.OrganizationUnitId == o.DestOrganizationUnitId)
                            || o.DestOrganizationUnit.ErmLaunchDate == null)),
                p.ValidationGroupPart);
            return overridenPredicate.GetCombinedPredicate();
        }

        protected override void Validate(ValidateOrdersRequest request, OrderValidationPredicate originalPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            if (request.Type != ValidationType.PreReleaseFinal && request.Type != ValidationType.ManualReportWithAccountsCheck)
            {   // No need to run this rule when peroforming manual check/technical release
                return;
            }

            var overridedPredicate = GetOrdersOverridedPredicate(request, originalPredicate);

            const int ApprovedLimitStatus = (int)LimitStatus.Approved;
            var epsilon = (decimal)Math.Pow(10, -request.SignificantDigitsNumber);

            _useCaseTuner.AlterDuration<BalanceOrderValidationRule>();
            var ordersPredicate = new OrderValidationPredicate(originalPredicate.GeneralPart, null, null).GetCombinedPredicate();
            var ordersForRelease = _finder.Find(ordersPredicate);
            var accountsWithInsufficientBalance = _finder.Find(overridedPredicate)
                .GroupBy(order => order.Account) // все лицевые счета для заказов участвующих в сборке
                .Select(x => new
                    {
                        AccountId = x.Key.Id,
                        AccountBalance = x.Key.Balance,

                        // Сумма одобренных лимитов лицевого счета за период.
                        LimitsSum = x.Key.Limits.Where(l => l.IsActive
                                                && !l.IsDeleted
                                                && l.Status == ApprovedLimitStatus
                                                && l.StartPeriodDate == request.Period.Start
                                                && l.EndPeriodDate == request.Period.End)
                                    .Sum(a => (decimal?)a.Amount) ?? 0M,

                        // Сумма активных блокировок лицевого счета
                        LocksSum = x.Key.Locks
                                        .Where(l => l.IsActive && !l.IsDeleted 
                                            && ((l.PeriodEndDate < request.Period.Start) || (l.PeriodStartDate > request.Period.End)))
                                        .Sum(l => (decimal?)l.PlannedAmount) ?? 0M,
                    })
                .GroupJoin( // докидываем для каждого лицевого счета все заказы по искомому лицевому счету, для любых отделений организации назначения заказа
                        ordersForRelease,
                        accountInfo => accountInfo.AccountId,
                        order => order.AccountId,
                        (accountInfo, accountOrders) =>
                            new
                            {
                                AccountResultBalance = accountInfo.AccountBalance + accountInfo.LimitsSum - accountInfo.LocksSum,
                                AccountResultNetBalanceWithoutLimits = accountInfo.AccountBalance - accountInfo.LocksSum,

                                // Сумма к текущему списанию по заказам.
                                OrdersAmountToWithdrawSum = accountOrders.Sum(order => order.OrderReleaseTotals
                                                                               .Where(a => a.ReleaseBeginDate == request.Period.Start
                                                                                        && a.ReleaseEndDate == request.Period.End)
                                                                               .Sum(a => (decimal?)a.AmountToWithdraw) ?? 0M),
                                OrderInfos = accountOrders.Select(o => new { o.Id, o.Number, o.SourceOrganizationUnitId, o.DestOrganizationUnitId })
                            })
                .Where(info => info.OrdersAmountToWithdrawSum > 0
                                && (info.AccountResultBalance < 0 || (info.OrdersAmountToWithdrawSum - info.AccountResultBalance) >= epsilon))
                .Select(info => new
                    {
                        info.AccountResultNetBalanceWithoutLimits,
                        AccountRequiredLimitAmount = info.OrdersAmountToWithdrawSum - info.AccountResultNetBalanceWithoutLimits,
                        info.OrdersAmountToWithdrawSum,
                        info.OrderInfos
                        })
                .ToArray();

            if (accountsWithInsufficientBalance.Any())
            {
                foreach (var accountInfo in accountsWithInsufficientBalance)
                {
                    foreach (var orderInfo in accountInfo.OrderInfos)
                    {
                        if (orderInfo.SourceOrganizationUnitId != request.OrganizationUnitId && orderInfo.DestOrganizationUnitId != request.OrganizationUnitId)
                        {   // в файл с ошибками не включаем записи о заказах, у которых город сборки не совпадает ни с городом назначеня, ни с городом источником заказа
                            // чтобы не перегружать излишей информацией запускальщика проверки
                            continue;
                        }

                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                MessageText = string.Format(BLResources.OrdersCheckOrderInsufficientFunds,
                                                            accountInfo.OrdersAmountToWithdrawSum,
                                                            accountInfo.AccountResultNetBalanceWithoutLimits,
                                                            accountInfo.AccountRequiredLimitAmount),
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number
                            });
                    }
                }
            }
        }
    }
}
