using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class LocalMessageViewModel : EntityViewModelBase<LocalMessage>
    {
        public DateTime? ProcessDate { get; set; }

        public IntegrationSystem SenderSystem { get; set; }

        public IntegrationSystem ReceiverSystem { get; set; }

        public LocalMessageStatus Status { get; set; }

        [DisplayNameLocalized("IntegrationType")]
        [Dependency(DependencyType.Hidden, "IntegrationTypeImport", "this.value==''")]
        public IntegrationTypeImport IntegrationTypeImport { get; set; }

        [DisplayNameLocalized("IntegrationType")]
        [Dependency(DependencyType.Hidden, "IntegrationTypeExport", "this.value==''")]
        public IntegrationTypeExport IntegrationTypeExport { get; set; }

        public LookupField OrganizationUnit { get; set; }

        public string ProcessResult { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LocalMessageDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            CreatedOn = modelDto.CreatedOn;
            ProcessDate = modelDto.EventDate;
            IntegrationTypeImport = modelDto.IntegrationTypeImport;
            IntegrationTypeExport = modelDto.IntegrationTypeExport;
            Status = modelDto.Status;
            SenderSystem = modelDto.SenderSystem;
            ReceiverSystem = modelDto.ReceiverSystem;
            ProcessResult = modelDto.ProcessResult;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {   
            throw new NotSupportedException();
        }
    }
}