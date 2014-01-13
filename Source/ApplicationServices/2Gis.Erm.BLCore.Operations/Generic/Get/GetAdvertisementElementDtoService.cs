using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementElementDtoService : GetDomainEntityDtoServiceBase<AdvertisementElement>
    {
        private readonly ISecureFinder _finder;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        public GetAdvertisementElementDtoService(IUserContext userContext, ISecureFinder finder, ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
            : base(userContext)
        {
            _finder = finder;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
        }

        protected override IDomainEntityDto<AdvertisementElement> GetDto(long entityId)
        {
            var dtoInfo = _finder.Find<AdvertisementElement>(x => x.Id == entityId)
                                 .Select(entity => new
                                     {
                                         Dto = new AdvertisementElementDomainEntityDto
                                             {
                                                 Id = entity.Id,
                                                 BeginDate = entity.BeginDate,
                                                 EndDate = entity.EndDate,
                                                 AdvertisementElementTemplateRef = new EntityReference { Id = entity.AdvertisementElementTemplate.Id, Name = entity.AdvertisementElementTemplate.Name },
                                                 TemplateFileExtensionRestriction = entity.AdvertisementElementTemplate.FileExtensionRestriction,
                                                 TemplateImageDimensionRestriction = entity.AdvertisementElementTemplate.ImageDimensionRestriction,
                                                 TemplateFormattedText = entity.AdvertisementElementTemplate.FormattedText,
                                                 TemplateAdvertisementLink = entity.AdvertisementElementTemplate.IsAdvertisementLink,
                                                 TemplateTextLengthRestriction = entity.AdvertisementElementTemplate.TextLengthRestriction,
                                                 TemplateMaxSymbolsInWord = entity.AdvertisementElementTemplate.MaxSymbolsInWord,
                                                 TemplateTextLineBreaksRestriction = entity.AdvertisementElementTemplate.TextLineBreaksCountRestriction,
                                                 TemplateRestrictionType = (AdvertisementElementRestrictionType)entity.AdvertisementElementTemplate.RestrictionType,
                                                 Status = (AdvertisementElementStatus)entity.Status,
                                                 Error = (AdvertisementElementError)entity.Error,

                                                 // заглушки не верифицируем
                                                 NeedsValidation = entity.AdvertisementElementTemplate.NeedsValidation && entity.Advertisement.FirmId != null,

                                                 FasCommentType = (FasComment)entity.FasCommentType,
                                                 PlainText = entity.Text,
                                                 FormattedText = entity.Text,
                                                 FileId = entity.FileId, 
                                                 FileName = entity.File != null ? entity.File.FileName : null, 
                                                 FileContentLength = entity.File != null ? entity.File.ContentLength : 0,
                                                 FileContentType = entity.File != null ? entity.File.ContentType : null,
                                                 OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                                 CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                 CreatedOn = entity.CreatedOn,
                                                 IsDeleted = entity.IsDeleted,
                                                 ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                 ModifiedOn = entity.ModifiedOn,
                                                 Timestamp = entity.Timestamp
                                             },
                                         FileTimestamp = entity.File != null ? entity.File.Timestamp : null,
                                         Template = entity.AdvertisementElementTemplate
                                     }).Single();

            dtoInfo.Dto.FileTimestamp = dtoInfo.FileTimestamp != null && dtoInfo.FileTimestamp.Length > 0
                                                ? Convert.ToBase64String(dtoInfo.FileTimestamp)
                                                : null;

            return dtoInfo.Dto;
        }

        protected override IDomainEntityDto<AdvertisementElement> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new AdvertisementElementDomainEntityDto();
        }

        protected override void SetDtoProperties(IDomainEntityDto<AdvertisementElement> domainEntityDto, long entityId, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;

            dto.CanUserChangeStatus = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification,
                                                                                                     UserContext.Identity.Code);
        }
    }
}