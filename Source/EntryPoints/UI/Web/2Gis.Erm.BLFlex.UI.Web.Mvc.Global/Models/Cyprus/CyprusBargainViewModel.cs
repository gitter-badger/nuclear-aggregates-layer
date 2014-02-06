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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus
{
    public sealed class CyprusBargainViewModel : EntityViewModelBase<Bargain>, ICyprusAdapted
    {
        #region Meaningful data

        [DisplayNameLocalized("BargainNumber")]
        public string Number { get; set; }

        [RequiredLocalized]
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

        [Dependency(DependencyType.Required, "DocumentsComment", "this.value == 'Other'")]
        public DocumentsDebt HasDocumentsDebt { get; set; }

        [StringLengthLocalized(300)]
        public string DocumentsComment { get; set; }

        #endregion

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Number = modelDto.Number;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            BranchOfficeOrganizationUnit = LookupField.FromReference(modelDto.BranchOfficeOrganizationUnitRef);
            Comment = modelDto.Comment;
            SignedOn = modelDto.SignedOn;
            ClosedOn = modelDto.ClosedOn;
            HasDocumentsDebt = modelDto.HasDocumentsDebt;
            DocumentsComment = modelDto.DocumentsComment;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (BargainType.Key == null)
            {
                throw new ArgumentNullException();
            }

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
                    LegalPersonRef = LegalPerson.ToReference(),
                    BranchOfficeOrganizationUnitRef = BranchOfficeOrganizationUnit.ToReference(),
                    Comment = Comment,
                    SignedOn = SignedOn,
                    ClosedOn = ClosedOn,
                    HasDocumentsDebt = HasDocumentsDebt,
                    DocumentsComment = DocumentsComment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}