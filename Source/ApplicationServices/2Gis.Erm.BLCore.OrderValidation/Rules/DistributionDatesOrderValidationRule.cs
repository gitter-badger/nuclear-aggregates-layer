using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить указанные даты размещения заказа
    /// </summary>
    public sealed class DistributionDatesOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public DistributionDatesOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
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
                        o.BeginDistributionDate,

                        // Окончание размещения = последнему дню месяца начала размещения + кол-во месяцев = Планируемому числу выпусков - 1 месяц
                        // Нет, у нас нету конвенции по поводу того, как хранить даты окончания периодов. Просто дляЗаказа это сейчас делается так: yyyy-mm-dd 23:59:59.000
                        IsEndDateValid = o.EndDistributionDatePlan == EntityFunctions.AddSeconds(EntityFunctions.AddMonths(o.BeginDistributionDate, o.ReleaseCountPlan), -1)
                    })
                    .Where(o => !o.IsEndDateValid || !o.IsBeginDateValid)
                    .ToList();

            foreach (var orderDetail in orderDetails)
            {
                if (!orderDetail.IsBeginDateValid)
                {
                    messages.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = BLResources.OrderCheckIncorrectBeginDistributionDate,
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }
                else
                    if (!orderDetail.IsEndDateValid)
                    {
                        messages.Add(new OrderValidationMessage
                                            {
                                                Type = MessageType.Error,
                                                MessageText = BLResources.OrderCheckIncorrectEndDistributionDate,
                                                OrderId = orderDetail.Id,
                                                OrderNumber = orderDetail.Number
                                            });
                    }
            }
        }
    }
}