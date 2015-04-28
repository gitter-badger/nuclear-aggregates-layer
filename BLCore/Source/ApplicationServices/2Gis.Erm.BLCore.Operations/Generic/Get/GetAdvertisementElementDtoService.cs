using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementElementDtoService : GetDomainEntityDtoServiceBase<AdvertisementElement>
    {
        private readonly IFinder _unsecureFinder;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISecurityServiceEntityAccessInternal _securityServiceEntityAccess;

        public GetAdvertisementElementDtoService(IUserContext userContext,
                                                 ISecureFinder secureFinder,
                                                 IFinder unsecureFinder,
                                                 ISecurityServiceEntityAccessInternal securityServiceEntityAccess,
                                                 ISecurityServiceFunctionalAccess securityServiceFunctionalAccess) : base(userContext)
        {
            _unsecureFinder = unsecureFinder;
            _secureFinder = secureFinder;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        protected override IDomainEntityDto<AdvertisementElement> GetDto(long entityId)
        {
            var dtoInfo = _secureFinder.Find<AdvertisementElement>(x => x.Id == entityId)
                                       .Select(entity => new
                                           {
                                               Dto = new AdvertisementElementDomainEntityDto
                                                   {
                                                       Id = entity.Id,
                                                       BeginDate = entity.BeginDate,
                                                       EndDate = entity.EndDate,
                                                       AdvertisementElementTemplateRef =
                                                           new EntityReference
                                                               {
                                                                   Id = entity.AdvertisementElementTemplate.Id,
                                                                   Name = entity.AdvertisementElementTemplate.Name
                                                               },
                                                       TemplateFileExtensionRestriction = entity.AdvertisementElementTemplate.FileExtensionRestriction,
                                                       TemplateImageDimensionRestriction = entity.AdvertisementElementTemplate.ImageDimensionRestriction,
                                                       TemplateFormattedText = entity.AdvertisementElementTemplate.FormattedText,
                                                       TemplateAdvertisementLink = entity.AdvertisementElementTemplate.IsAdvertisementLink,
                                                       TemplateTextLengthRestriction = entity.AdvertisementElementTemplate.TextLengthRestriction,
                                                       TemplateMaxSymbolsInWord = entity.AdvertisementElementTemplate.MaxSymbolsInWord,
                                                       TemplateTextLineBreaksRestriction = entity.AdvertisementElementTemplate.TextLineBreaksCountRestriction,
                                                       TemplateRestrictionType =
                                                           entity.AdvertisementElementTemplate.RestrictionType,

                                                       Status = (AdvertisementElementStatusValue)entity.AdvertisementElementStatus.Status,

                                                       // заглушки не верифицируем
                                                       NeedsValidation =
                                                           entity.AdvertisementElementTemplate.NeedsValidation && entity.Advertisement.FirmId != null,
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
                                               Template = entity.AdvertisementElementTemplate
                                           }).Single();

            // В данном случае намеренно используется небезопасная версия файндера
            var firmInfo = _unsecureFinder
                .Find(Specs.Find.ById<AdvertisementElement>(entityId))
                .Select(x => x.Advertisement.Firm)
                .Where(x => x != null)
                .Select(x => new
                    {
                        x.Id,
                        x.OwnerCode
                    })
                .SingleOrDefault();

            // для заглушек вместо прав на фирму проверяем функциональную привилегию
            dtoInfo.Dto.SetReadonly = firmInfo != null
                                          ? !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                          EntityName.Firm,
                                                                                          UserContext.Identity.Code,
                                                                                          firmInfo.Id,
                                                                                          firmInfo.OwnerCode,
                                                                                          firmInfo.OwnerCode)
                                          : !_securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditDummyAdvertisement,
                                                                                                            UserContext.Identity.Code);

            return dtoInfo.Dto;
        }

        protected override IDomainEntityDto<AdvertisementElement> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new AdvertisementElementDomainEntityDto();
        }

        protected override void SetDtoProperties(
            IDomainEntityDto<AdvertisementElement> domainEntityDto, 
            long entityId, 
            bool readOnly, 
            long? parentEntityId, 
            EntityName parentEntityName, 
            string extendedInfo)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;
            dto.CanUserChangeStatus = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, UserContext.Identity.Code);
        }
    }
}