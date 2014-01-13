using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    [Obsolete("Проверка отключена в рамках тикета ERM-2342. Пока решено только отключить. Возможно, в будущем ее можно будет удалить")]
    public sealed class AdvantageousPurchasesFirmsHasBannerOrCommentValidationRule : OrderValidationRuleCommonPredicate
    {
        private const int AdvantageousPurchasesRubricId = 18599;
        private const int BannerItemId = 23; // Баннер в рубрике
        private const int Banner2GisItemId = 296; // Баннер в рубрике Выгодные покупки с 2ГИС
        private const int CommentaryCollapsed = 30; // Микрокомментарий в рубрике(Микрокомментарий к свёрнутой карточке в справочнике)
        private const int CommentaryTransport = 7; // Микрокомментарий в транспорте
        private const int BannerAttachItemId = 2; // Подключение баннера к рубрике
        private const int CommentaryAttachItemId = 40; // Подключение микрокомментария к дополнительной рубрике

        private readonly IFinder _finder;

        public AdvantageousPurchasesFirmsHasBannerOrCommentValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var period = IsCheckMassive
                             ? request.Period
                             : GetOrderPeriod(request.OrderId);

            var invalidRubricUsages = _finder.Find(filterPredicate)
                
                // Фирмы, имеющие заказы в проверке
                .Select(order => order.Firm)
                
                // Те из них, что имеют рубрику "Выгодные покупки"
                .Where(firm => firm.FirmAddresses.Any(address => address.IsActive && !address.IsDeleted &&
                    address.CategoryFirmAddresses.Any(category => category.IsActive && !category.IsDeleted && category.CategoryId == AdvantageousPurchasesRubricId)))
                
                // Теперь оставляем только те фирмы, что не имеют нужных позиций в действующих заказах проверяемого периода
                .Where(firm => !firm.Orders.Any(order => order.IsActive && !order.IsDeleted
                                            && (order.Id == request.OrderId || order.WorkflowStepId == (int)OrderState.OnApproval || order.WorkflowStepId == (int)OrderState.Approved || order.WorkflowStepId == (int)OrderState.OnTermination)
                                            && (order.EndDistributionDateFact >= period.End && order.BeginDistributionDate <= period.Start)
                                            && order.OrderPositions
                                                    .Where(position => position.IsActive && !position.IsDeleted)
                                                    .SelectMany(position => position.OrderPositionAdvertisements)
                                                    .Where(advertisement => advertisement.CategoryId == AdvantageousPurchasesRubricId)
                                                    .Select(advertisement => advertisement.Position.PositionCategory)
                                                    .Any(category => category.Id == BannerItemId 
                                                                     || category.Id == Banner2GisItemId 
                                                                     || category.Id == CommentaryCollapsed
                                                                     || category.Id == CommentaryTransport
                                                                     || category.Id == BannerAttachItemId
                                                                     || category.Id == CommentaryAttachItemId)))
                .Select(firm => new { Id = firm.Id, Name = firm.Name })
                .Distinct()
                .OrderBy(dto => dto.Name)
                .ToArray();

            var rubricName = _finder
                .Find(Specs.Find.ById<Category>(AdvantageousPurchasesRubricId))
                .Select(category => new { category.Name, category.Id })
                .Single();

            foreach (var firmName in invalidRubricUsages)
            {
                var firmDescription = GenerateDescription(EntityName.Firm, firmName.Name, firmName.Id);
                var rubricDescription = GenerateDescription(EntityName.Category, rubricName.Name, rubricName.Id);

                messages.Add(new OrderValidationMessage
                {
                    Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                    MessageText = string.Format(BLResources.InvalidFirmRubricUsage, firmDescription, rubricDescription),
                });
            }
        }

        private TimePeriod GetOrderPeriod(long? orderId)
        {
            if (!orderId.HasValue)
            {
                throw new ArgumentNullException("orderId");
            }

            var periodDto = _finder.Find(Specs.Find.ById<Order>(orderId.Value))
                          .Select(order => new { order.BeginDistributionDate, order.EndDistributionDateFact })
                          .Single();

            return new TimePeriod(periodDto.BeginDistributionDate, periodDto.EndDistributionDateFact);
        }
    }
}
