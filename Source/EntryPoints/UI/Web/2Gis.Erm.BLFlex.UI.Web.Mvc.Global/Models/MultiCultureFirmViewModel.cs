using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureFirmViewModel : EntityViewModelBase<Firm>, ICustomizableFirmViewModel, ICyprusAdapted, IChileAdapted, ICzechAdapted, IUkraineAdapted,
                                                    IEmiratesAdapted, IKazakhstanAdapted
    {
        [DisplayNameLocalized("FirmName")]
        public string Name { get; set; }

        [DisplayNameLocalized("IsHiddenFemale")]
        [YesNoRadio]
        public bool ClosedForAscertainment { get; set; }

        public string Comment { get; set; }

        // ���� ������ �� �������
        public DateTime? LastQualifyTime { get; set; }

        // ���� �������� � ������
        public DateTime? LastDisqualifyTime { get; set; }

        // ������
        public LookupField Client { get; set; }

        public string ClientName { get; set; }

        // ����������
        [RequiredLocalized]
        public LookupField Territory { get; set; }

        // �����
        public LookupField OrganizationUnit { get; set; }

        // �������
        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (FirmDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            ClosedForAscertainment = modelDto.ClosedForAscertainment;
            Comment = modelDto.Comment;

            LastQualifyTime = modelDto.LastQualifyTime;
            LastDisqualifyTime = modelDto.LastDisqualifyTime;

            Client = LookupField.FromReference(modelDto.ClientRef);

            Territory = LookupField.FromReference(modelDto.TerritoryRef);
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new FirmDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    ClosedForAscertainment = ClosedForAscertainment,
                    Comment = Comment,

                    LastQualifyTime = LastQualifyTime,
                    LastDisqualifyTime = LastDisqualifyTime,

                    ClientRef = Client.ToReference(),

                    TerritoryRef = Territory.ToReference(),

                    OwnerRef = Owner.ToReference(),

                    Timestamp = Timestamp
                };

            return dto;
        }
    }
}