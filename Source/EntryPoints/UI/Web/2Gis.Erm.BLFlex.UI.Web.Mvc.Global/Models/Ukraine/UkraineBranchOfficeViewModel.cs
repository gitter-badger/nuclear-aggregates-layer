using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public sealed class UkraineBranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, IUkraineAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [StringLengthLocalized(12, MinimumLength = 10)]
        public string Ipn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(10, MinimumLength = 8)]
        public string Egrpou { get; set; }

        [RequiredLocalized]
        public LookupField BargainType { get; set; }

        [RequiredLocalized]
        public LookupField ContributionType { get; set; }

        public string Annotation { get; set; }

        public string UsnNotificationText { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (UkraineBranchOfficeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Ipn = modelDto.Ipn;
            Egrpou = modelDto.Inn; // ����, ������ �������� � Inn, �.�. ������ �� ������������ � �� ����� �������� ����������������� �������
            Annotation = modelDto.Annotation;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            ContributionType = LookupField.FromReference(modelDto.ContributionTypeRef);
            LegalAddress = modelDto.LegalAddress;
            UsnNotificationText = modelDto.UsnNotificationText;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new UkraineBranchOfficeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Inn = Egrpou, // ����, ������ �������� � Inn, �.�. ������ �� ������������ � �� ����� �������� ����������������� �������
                    Ipn = Ipn,
                    Annotation = Annotation,
                    BargainTypeRef = BargainType.ToReference(),
                    ContributionTypeRef = ContributionType.ToReference(),
                    LegalAddress = LegalAddress,
                    UsnNotificationText = UsnNotificationText,
                    Timestamp = Timestamp
                };
        }
    }
}