using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    public sealed class ADPValidationQueryProvider
    {
        private readonly IFinder _finder;
        private readonly ADPCheckMode _checkMode;
        private readonly long _orderId;
        private readonly Expression<Func<Order, bool>> _filterExpression;

        public ADPValidationQueryProvider(IFinder finder, ADPCheckMode checkMode, long orderId, Expression<Func<Order, bool>> filterExpression)
        {
            _finder = finder;
            _checkMode = checkMode;
            _orderId = orderId;
            _filterExpression = filterExpression;
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            switch (_checkMode)
            {
                case ADPCheckMode.OrderBeingCancelled:
                case ADPCheckMode.OrderBeingReapproved:
                case ADPCheckMode.SpecificOrder:
                    var orderInfo = _finder.Find(Specs.Find.ById<Order>(_orderId))
                        .Select(item => new
                            {
                                item.FirmId,
                                item.BeginReleaseNumber,
                                item.DestOrganizationUnitId,
                                EndReleaseNumber = _checkMode == ADPCheckMode.OrderBeingReapproved
                                                       ? item.EndReleaseNumberPlan //После возвращения, Fact сбросится на Plan
                                                       : item.EndReleaseNumberFact
                            }).Single();
                    Expression<Func<Order, bool>> orderFilter =
                        order => order.IsActive
                                 && !order.IsDeleted
                                 && order.FirmId == orderInfo.FirmId
                                 && order.DestOrganizationUnitId == orderInfo.DestOrganizationUnitId
                                 && (order.Id == _orderId
                                     || order.WorkflowStepId == OrderState.OnApproval
                                     || order.WorkflowStepId == OrderState.Approved
                                     || order.WorkflowStepId == OrderState.OnTermination)
                                 && ((order.BeginReleaseNumber >= orderInfo.BeginReleaseNumber && order.BeginReleaseNumber <= orderInfo.EndReleaseNumber)
                                     || (order.EndReleaseNumberFact >= orderInfo.BeginReleaseNumber && order.EndReleaseNumberFact <= orderInfo.EndReleaseNumber)
                                     || (orderInfo.BeginReleaseNumber >= order.BeginReleaseNumber && orderInfo.BeginReleaseNumber <= order.EndReleaseNumberFact)
                                     || (orderInfo.EndReleaseNumber >= order.BeginReleaseNumber && orderInfo.EndReleaseNumber <= order.EndReleaseNumberFact));

                    return _finder.Find(orderFilter);

                case ADPCheckMode.Massive:
                    // Находим заказы, для фирм которых оформлено больше одного заказа,
                    // т.к. если для фирмы оформлен всего один заказ, то ошибки в нем будут учтены при единичной проверке
                    var moreThanOneOrderForFirmQuery = from order in _finder.For<Order>()
                                                       where !order.IsDeleted
                                                       group order.Id by order.FirmId
                                                       into orderIdsByFirm
                                                       where orderIdsByFirm.Count() > 1
                                                       from orderId in orderIdsByFirm
                                                       select orderId;
                    // Берем для проверки только те заказы, для фирм которых оформлено больше одного заказа
                    return from order in _finder.Find(_filterExpression)
                           join orderId in moreThanOneOrderForFirmQuery on order.Id equals orderId
                           select order;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IQueryable<OrderPosition> GetOrderPositionsQuery()
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrderPosition>());
        }

        public IQueryable<DoubleGis.Erm.Platform.Model.Entities.Erm.Category> GetCategoryQuery()
        {
            return _finder.For<DoubleGis.Erm.Platform.Model.Entities.Erm.Category>();
        }
    }
}
