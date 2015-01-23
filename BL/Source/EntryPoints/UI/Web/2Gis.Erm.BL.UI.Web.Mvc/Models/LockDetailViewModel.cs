using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class LockDetailViewModel : EntityViewModelBase<LockDetail>
    {
        // Дата создания
        [DisplayNameLocalized("LockDetailCreateDate")]
        public DateTime CreateDate { get; set; }

        // Сумма
        public decimal Amount { get; set; }

        // Прайс-лист
        [RequiredLocalized]
        [DisplayNameLocalized("PriceList")]
        public LookupField Price { get; set; }

        public LookupField OrderPosition { get; set; }

        // Описание
        [StringLengthLocalized(200)]
        public string Description { get; set; }

        public long LockId { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LockDetailDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            CreateDate = modelDto.CreatedOn;
            Amount = modelDto.Amount;
            Price = LookupField.FromReference(modelDto.PriceRef);
            OrderPosition = LookupField.FromReference(modelDto.OrderPositionRef);
            Description = modelDto.Description;
            LockId = modelDto.LockRef.Id.Value;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LockDetailDomainEntityDto
                {
                    Id = Id,
                    CreatedOn = CreateDate,
                    Amount = Amount,
                    PriceRef = Price.ToReference(),
                    Description = Description,
                    LockRef = new EntityReference(LockId),
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}