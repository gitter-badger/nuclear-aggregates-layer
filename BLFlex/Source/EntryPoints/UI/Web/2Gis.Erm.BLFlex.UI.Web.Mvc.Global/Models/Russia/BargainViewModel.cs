using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class BargainViewModel : EntityViewModelBase<Bargain>, IRussiaAdapted, INumberAspect
    {
        #region Meaningful data

        [DisplayNameLocalized("BargainNumber")]
        public string Number { get; set; }

        public LookupField BargainType { get; set; }

        [DisplayNameLocalized("BargainLegalPerson")]
        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        [RequiredLocalized]
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        public string Comment { get; set; }

        [RequiredLocalized]
        public DateTime SignedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        [DisplayNameLocalized("BargainEndDate")]
        public DateTime? BargainEndDate { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "BargainEndDate", "this.value=='Agent'")]
        [Dependency(DependencyType.Disable, "BargainKind",
            "Ext.getDom('Id').value != '0'||Ext.getDom('UserCanWorkWithAdvertisingAgencies').value.toLowerCase() == 'false'")]
        public BargainKind BargainKind { get; set; }

        public bool UserCanWorkWithAdvertisingAgencies { get; set; }

        [Dependency(DependencyType.ReadOnly, "LegalPerson", "Ext.getDom('Id').value != '0'||this.value.toLowerCase() == 'true'")]
        public bool IsLegalPersonChoosingDenied { get; set; }

        [Dependency(DependencyType.ReadOnly, "BranchOfficeOrganizationUnit", "Ext.getDom('Id').value != '0'||this.value.toLowerCase() == 'true'")]
        public bool IsBranchOfficeOrganizationUnitChoosingDenied { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        [Dependency(DependencyType.Required, "DocumentsComment", "this.value == 'Other'")]
        public DocumentsDebt HasDocumentsDebt { get; set; }

        [StringLengthLocalized(300)]
        public string DocumentsComment { get; set; }

        public long ClientId { get; set; }

        #endregion

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Number = modelDto.Number;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            LegalPerson = LookupField.FromReference(modelDto.CustomerLegalPersonRef);
            BranchOfficeOrganizationUnit = LookupField.FromReference(modelDto.ExecutorBranchOfficeRef);
            Comment = modelDto.Comment;
            SignedOn = modelDto.SignedOn;
            ClosedOn = modelDto.ClosedOn;
            BargainEndDate = modelDto.BargainEndDate;
            BargainKind = modelDto.BargainKind;
            HasDocumentsDebt = modelDto.HasDocumentsDebt;
            DocumentsComment = modelDto.DocumentsComment;
            Timestamp = modelDto.Timestamp;
            UserCanWorkWithAdvertisingAgencies = false;
            IsLegalPersonChoosingDenied = modelDto.IsLegalPersonChoosingDenied;
            IsBranchOfficeOrganizationUnitChoosingDenied = modelDto.IsBranchOfficeOrganizationUnitChoosingDenied;
            ClientId = modelDto.ClientId;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (LegalPerson.Key == null)
            {
                throw new ArgumentNullException();
            }

            if (BranchOfficeOrganizationUnit.Key == null)
            {
                throw new ArgumentNullException();
            }

            return new BargainDomainEntityDto
                {
                    Id = Id,
                    Number = Number,
                    BargainTypeRef = BargainType.ToReference(),
                    CustomerLegalPersonRef = LegalPerson.ToReference(),
                    ExecutorBranchOfficeRef = BranchOfficeOrganizationUnit.ToReference(),
                    Comment = Comment,
                    SignedOn = SignedOn,
                    ClosedOn = ClosedOn,
                    BargainEndDate = BargainEndDate,
                    BargainKind = BargainKind,
                    HasDocumentsDebt = HasDocumentsDebt,
                    DocumentsComment = DocumentsComment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp,
                    UserCanWorkWithAdvertisingAgencies = false,
                    IsLegalPersonChoosingDenied = IsLegalPersonChoosingDenied,
                    IsBranchOfficeOrganizationUnitChoosingDenied = IsBranchOfficeOrganizationUnitChoosingDenied,
                    ClientId = ClientId
                };
        }
    }
}