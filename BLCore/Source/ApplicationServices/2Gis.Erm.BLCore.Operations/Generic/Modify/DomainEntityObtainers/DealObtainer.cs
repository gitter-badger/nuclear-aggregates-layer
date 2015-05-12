using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class DealObtainer : IBusinessModelEntityObtainer<Deal>, IAggregateReadModel<Deal>
    {
        private readonly IFinder _finder;

        public DealObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Deal ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DealDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<Deal>(dto.Id)).SingleOrDefault() ??
                         new Deal { IsActive = true };

            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.Comment = dto.Comment;
            entity.StartReason = dto.StartReason;
            entity.MainFirmId = dto.MainFirmRef.Id;
            entity.DealStage = dto.DealStage;
            entity.ClientId = dto.ClientRef.Id.Value;
            entity.OwnerCode = dto.OwnerRef.Id.Value;

            // Рекламная кампания
            entity.AdvertisingCampaignBeginDate = dto.AdvertisingCampaignBeginDate;
            entity.AdvertisingCampaignEndDate = dto.AdvertisingCampaignEndDate;
            entity.AdvertisingCampaignGoalText = dto.AdvertisingCampaignGoalText;
            entity.AdvertisingCampaignGoals = dto.AdvertisingCampaignGoals;
            entity.AgencyFee = dto.AgencyFee;
            entity.BargainId = dto.BargainRef.Id;
            entity.PaymentFormat = (int?)dto.PaymentFormat;

            // Параметры сделки могут быть изменены при редактировании заказа
            // Если были открыты карточкы сделки и заказа и заказ был изменен -> сериализованный объект в карточке сделки становится невалидным,
            // т.к. в хранилище данных этот объект был изменен
            // Таким образом, при попытке сохранить сделку получим "искусственную" оптимистичную блокировку
            // Это говорит о том, что сериализованный объект в карточке сделки нужно обновлять при редактировании любого заказа этой сделки,
            // либо всегда обновлять состояние сделки перед применением изменений к ней 
            // -> тем самым отключив поддержку оптимистичных блокировок при сохранении корректности логики
            // Ниже реализован второй вариант. При реализации новой клиентской части ERM нужно учесть эти кейсы (позиция заказа - заказ и заказ - сделка)
            if (dto.Id == 0)
            {
                entity.Timestamp = dto.Timestamp;
            }

            return entity;
        }
    }
}