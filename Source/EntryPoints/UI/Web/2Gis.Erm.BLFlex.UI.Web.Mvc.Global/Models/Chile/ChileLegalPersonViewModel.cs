using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileLegalPersonViewModel : EntityViewModelBase<LegalPerson>, IChileAdapted
    {
        public Guid? ReplicationCode { get; set; }
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        public LegalPersonType LegalPersonType { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string ActivityType { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "Rut", "Ext.getDom('Id').value != '0'")]
        [RutValidation]
        public string Rut { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        public string LegalAddress { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        [Dependency(DependencyType.ReadOnly, "Commune", "Ext.getDom('Id').value != '0'")]
        public LookupField Commune { get; set; }
        
        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public bool HasProfiles { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            LegalName = modelDto.LegalName;
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            Rut = modelDto.Inn;
            Client = LookupField.FromReference(modelDto.ClientRef);
            Commune = LookupField.FromReference(new EntityReference(null)); // FIXME {all, 24.01.2014}: Читать значение коммуны
            ReplicationCode = modelDto.ReplicationCode;
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            // FIXME {all, 24.01.2014}: Сохранять значение коммуны
            return new LegalPersonDomainEntityDto
                {
                    Id = Id,
                    LegalName = LegalName,
                    ShortName = LegalName,
                    LegalPersonTypeEnum = LegalPersonType,
                    LegalAddress = LegalAddress,
                    Inn = Rut,
                    ClientRef = Client.ToReference(),
                    ReplicationCode = ReplicationCode.Value,
                    Comment = Comment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}