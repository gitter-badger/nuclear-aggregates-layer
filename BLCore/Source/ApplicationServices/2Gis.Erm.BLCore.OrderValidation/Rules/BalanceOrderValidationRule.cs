using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Storage.UseCases;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка достаточности денежных средств на лицевом счете для размещения заказа
    /// </summary>
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class BalanceOrderValidationRule : OrderValidationRuleBase<MassOverridibleValidationRuleContext>
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IQuery _query;
        private readonly IUseCaseTuner _useCaseTuner;

        public BalanceOrderValidationRule(IBusinessModelSettings businessModelSettings, IQuery query, IUseCaseTuner useCaseTuner)
        {
            _businessModelSettings = businessModelSettings;
            _query = query;
            _useCaseTuner = useCaseTuner;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(MassOverridibleValidationRuleContext ruleContext)
        {
            var overridedPredicate = GetOrdersOverridedPredicate(ruleContext.ValidationParams, ruleContext.CombinedPredicate);

            var epsilon = (decimal)Math.Pow(10, -_businessModelSettings.SignificantDigitsNumber);

            _useCaseTuner.AlterDuration<BalanceOrderValidationRule>();
            var ordersPredicate = new OrderValidationPredicate(ruleContext.CombinedPredicate.GeneralPart, null, null).GetCombinedPredicate();
            var ordersForRelease = _query.For(new FindSpecification<Order>(ordersPredicate));
            var accountsWithInsufficientBalance = _query.For(new FindSpecification<Order>(overridedPredicate))
                .GroupBy(order => order.Account) // все лицевые счета для заказов участвующих в сборке
                .Select(x => new
                    {
                        AccountId = x.Key.Id,
                        AccountBalance = x.Key.Balance,

                        // Сумма одобренных лимитов лицевого счета за период.
                        LimitsSum = x.Key.Limits.Where(l => l.IsActive
                                                && !l.IsDeleted
                                            && l.Status == LimitStatus.Approved
                                            && l.StartPeriodDate == ruleContext.ValidationParams.Period.Start
                                            && l.EndPeriodDate == ruleContext.ValidationParams.Period.End)
                                    .Sum(a => (decimal?)a.Amount) ?? 0M,

                        // Сумма активных блокировок лицевого счета
                        LocksSum = x.Key.Locks
                                        .Where(l => l.IsActive && !l.IsDeleted 
                                        && ((l.PeriodEndDate < ruleContext.ValidationParams.Period.Start) || (l.PeriodStartDate > ruleContext.ValidationParams.Period.End)))
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
                                                                               .Where(a => a.ReleaseBeginDate == ruleContext.ValidationParams.Period.Start
                                                                                        && a.ReleaseEndDate == ruleContext.ValidationParams.Period.End)
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

            return from accountInfo in accountsWithInsufficientBalance
                   from orderInfo in accountInfo.OrderInfos
                   // в файл с ошибками не включаем записи о заказах, у которых город сборки не совпадает ни с городом назначеня, ни с городом источником заказа
                            // чтобы не перегружать излишей информацией запускальщика проверки
                   where orderInfo.SourceOrganizationUnitId == ruleContext.ValidationParams.OrganizationUnitId
                         || orderInfo.DestOrganizationUnitId == ruleContext.ValidationParams.OrganizationUnitId
                   select new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                  MessageText =
                                      string.Format(BLResources.OrdersCheckOrderInsufficientFunds,
                                                            accountInfo.OrdersAmountToWithdrawSum,
                                                            accountInfo.AccountResultNetBalanceWithoutLimits,
                                                            accountInfo.AccountRequiredLimitAmount),
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number
                              };
            }

        // Выборка заказов происходит в 2 этапа. Этот метод вернет предикат для заказов, город ИСТОЧНИК которых равен городу, выбранному в форме
        // Далее при валидации накладывается еще один фильтр для заказов, по которым для города НАЗНАЧЕНИЯ не было финальной сборки за период, либо 
        //                                                                               город НАЗНАЧЕНИЯ не переведен на ERM
        // Итого для этой проверки получаем такой фильтр: 
        // Заказы, для которых город ИСТОЧНИК равен городу, выбранному в форме 
        // И ( [по городу НАЗНАЧЕНИЯ не было финальной сборки за период] ИЛИ [город НАЗНАЧЕНИЯ не переведен на ERM] )
        private Expression<Func<Order, bool>> GetOrdersOverridedPredicate(MassOrdersValidationParams validationParams, OrderValidationPredicate predicate)
        {
            var finalReleaseInfos =
                _query.For(ReleaseSpecs.Releases.Find.FinalForPeriodWithStatuses(new TimePeriod(validationParams.Period.Start, validationParams.Period.End), ReleaseStatus.Success));

            var overridenPredicate = new OrderValidationPredicate(
                predicate.GeneralPart,
                o => (o.DestOrganizationUnitId == validationParams.OrganizationUnitId
                        && !finalReleaseInfos.Any(x => x.OrganizationUnitId == o.SourceOrganizationUnitId))
                    || (o.SourceOrganizationUnitId == validationParams.OrganizationUnitId
                        && (!finalReleaseInfos.Any(x => x.OrganizationUnitId == o.DestOrganizationUnitId)
                            || o.DestOrganizationUnit.ErmLaunchDate == null)),
                predicate.ValidationGroupPart);
            return overridenPredicate.GetCombinedPredicate();
        }
    }
}
