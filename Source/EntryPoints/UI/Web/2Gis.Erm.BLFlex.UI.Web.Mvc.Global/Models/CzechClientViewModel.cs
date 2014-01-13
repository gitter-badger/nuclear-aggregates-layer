using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    // 2+: BLFlex\Source\EntryPoints\UI\Web\2Gis.Erm.BLFlex.Web.Mvc.Global\Models\CzechClientViewModel.cs
    public sealed class CzechClientViewModel : EntityViewModelBase<Client>, ICzechAdapted
    {
        [PresentationLayerProperty]
        public Guid? ReplicationCode { get; set; }

        // Наименование
        [StringLengthLocalized(250)]
        [RequiredLocalized]
        [DisplayNameLocalized("NameOfClient")]
        public string Name { get; set; }

        [StringLengthLocalized(512)]
        public string MainAddress { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        // Основной телефон
        [StringLengthLocalized(64)]
        public string MainPhoneNumber { get; set; }

        // Дополнительный телефон 1
        [StringLengthLocalized(64)]
        public string AdditionalPhoneNumber1 { get; set; }

        // Дополнительный телефон 2
        [StringLengthLocalized(64)]
        public string AdditionalPhoneNumber2 { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(100)]
        public string Email { get; set; }

        // Факс
        [StringLengthLocalized(50)]
        public string Fax { get; set; }

        // Веб-сайт
        [UrlLocalized]
        [StringLengthLocalized(200)]
        public string Website { get; set; }

        // Основная фирма
        public LookupField MainFirm { get; set; }

        // Территория
        [RequiredLocalized]
        public LookupField Territory { get; set; }

        // Источник (enum) - источник показывает как клиент попал к нам - через сми, через отдел продаж, etc
        [RequiredLocalized]
        public InformationSource InformationSource { get; set; }

        // Дата взятия из резерва
        public DateTime LastQualifyTime { get; set; }

        // Дата возврата в резерв
        public DateTime? LastDisqualifyTime { get; set; }

        // Дата возврата в резерв
        public bool IsAdvertisingAgency { get; set; }

        [Dependency(DependencyType.ReadOnly, "IsAdvertisingAgency", "this.value == 'False'")]
        public bool CanEditIsAdvertisingAgency { get; set; }

        // Куратор
        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        // Поле, необходимое для мерджа клиентов
        public long AppendedClient { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ClientDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            MainPhoneNumber = modelDto.MainPhoneNumber;
            AdditionalPhoneNumber1 = modelDto.AdditionalPhoneNumber1;
            AdditionalPhoneNumber2 = modelDto.AdditionalPhoneNumber2;
            Email = modelDto.Email;
            Fax = modelDto.Fax;
            Website = modelDto.Website;
            InformationSource = modelDto.InformationSource;
            ReplicationCode = modelDto.ReplicationCode;
            Comment = modelDto.Comment;
            MainAddress = modelDto.MainAddress;
            IsAdvertisingAgency = modelDto.IsAdvertisingAgency;
            LastQualifyTime = modelDto.LastQualifyTime;
            LastDisqualifyTime = modelDto.LastDisqualifyTime;
            MainFirm = LookupField.FromReference(modelDto.MainFirmRef);
            Territory = LookupField.FromReference(modelDto.TerritoryRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new ClientDomainEntityDto
            {
                Id = Id,
                Name = Name,
                MainPhoneNumber = MainPhoneNumber,
                AdditionalPhoneNumber1 = AdditionalPhoneNumber1,
                AdditionalPhoneNumber2 = AdditionalPhoneNumber2,
                Email = Email,
                Fax = Fax,
                Website = Website,
                InformationSource = InformationSource,
                Comment = Comment,
                MainAddress = MainAddress,
                LastQualifyTime = LastQualifyTime,
                IsAdvertisingAgency = IsAdvertisingAgency,
                LastDisqualifyTime = LastDisqualifyTime,
                MainFirmRef = MainFirm.ToReference(),
                OwnerRef = Owner.ToReference(),
                Timestamp = Timestamp
            };

            if (Territory != null && Territory.Key != null)
            {
                dto.TerritoryRef = Territory.ToReference();
            }

            return dto;
        }
    }
}