using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить указанные даты размещения заказа
    /// </summary>
    public sealed class DistributionDatesOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public DistributionDatesOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var orderDetails = _query.For<Order>()
                    .Where(ruleContext.OrdersFilterPredicate)
                    .Select(o => new
                        {
                            o.Id,
                            o.Number,
                            o.BeginDistributionDate,
                            o.EndDistributionDatePlan,
                            o.ReleaseCountPlan,
                        })
                    .Select(o => new
                    {
                        o.Id,
                        o.Number,

                        IsBeginDateValid = o.BeginDistributionDate.Day == 1, // проверяем только, что первый день месяца

                        // Окончание размещения = последнему дню месяца начала размещения + кол-во месяцев = Планируемому числу выпусков - 1 месяц
                        // Нет, у нас нету конвенции по поводу того, как хранить даты окончания периодов. Просто дляЗаказа это сейчас делается так: yyyy-mm-dd 23:59:59.000
                        IsEndDateValid = o.EndDistributionDatePlan == DbFunctions.AddSeconds(DbFunctions.AddMonths(o.BeginDistributionDate, o.ReleaseCountPlan), -1)
                    })
                    .Where(o => !o.IsEndDateValid || !o.IsBeginDateValid)
                    .ToList();

            var results = new List<OrderValidationMessage>();

            foreach (var orderDetail in orderDetails)
            {
                if (!orderDetail.IsBeginDateValid)
                {
                    results.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = BLResources.OrderCheckIncorrectBeginDistributionDate,
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }
                else if (!orderDetail.IsEndDateValid)
                {
                    results.Add(new OrderValidationMessage
                                        {
                                            Type = MessageType.Error,
                                            MessageText = BLResources.OrderCheckIncorrectEndDistributionDate,
                                            OrderId = orderDetail.Id,
                                            OrderNumber = orderDetail.Number
                                        });
                }
            }

            return results;
        }
    }
}