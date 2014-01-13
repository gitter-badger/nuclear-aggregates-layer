using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals
{
    // TODO {all, 15.09.2013}: явно нарушается SRP - выполняет и валидацию и смену стадии сделки - учесть при рефакторинге ApplicationServices
    public sealed class CheckDealHandler : RequestHandler<CheckDealRequest, EmptyResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;

        public CheckDealHandler(
            IDealReadModel dealReadModel,
            IDealChangeStageAggregateService dealChangeStageAggregateService)
        {
            _dealReadModel = dealReadModel;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
        }

        protected override EmptyResponse Handle(CheckDealRequest request)
        {
            var deal = _dealReadModel.GetDeal(request.DealId);
            if (deal == null)
            {
                throw new NotificationException(string.Format(BLResources.DealCodeInvalid, request.DealId));
            }

            // http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=48465270
            //  => 6'. У заказа есть связный документ "Сделка" и Сделка.Валюта != Заказ.Валюта -> Вызвать окно уведомления с текстом "Необходимо выбрать город источник соответствующий валюте сделки". (Исполнитель: CRM::Билинг).
            if (deal.CurrencyId != request.CurrencyId)
            {
                throw new NotificationException(BLResources.OrderHasAssignedIncorrectCurrency);
            }

            if (!_dealReadModel.HasOrders(request.DealId))
            {
                // сохраняется только первый заказ у сделки => "В документе "Сделка" изменить этап на «Сформирован заказ»."
                _dealChangeStageAggregateService.ChangeStage(new[] { new DealChangeStageDto { Deal = deal, NextStage = DealStage.OrderFormed } });
            }

            return Response.Empty;
        }
    }
}
