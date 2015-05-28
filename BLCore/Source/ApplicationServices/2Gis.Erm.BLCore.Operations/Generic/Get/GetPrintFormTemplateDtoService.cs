using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPrintFormTemplateDtoService : GetDomainEntityDtoServiceBase<PrintFormTemplate>
    {
        private readonly ISecureFinder _finder;

        public GetPrintFormTemplateDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<PrintFormTemplate> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<PrintFormTemplate>(x => x.Id == entityId))
                          .Select(entity => new PrintFormTemplateDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    BranchOfficeOrganizationUnitRef = new EntityReference { Id = entity.BranchOfficeOrganizationUnitId, Name = null },
                                                    FileId = entity.FileId, 
                                                    FileName = entity.File.FileName,
                                                    FileContentType = entity.File.ContentType,
                                                    FileContentLength = entity.File.ContentLength,
                                                    TemplateCode = entity.TemplateCode,
                                                    OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                    ModifiedOn = entity.ModifiedOn,
                                                    Timestamp = entity.Timestamp
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<PrintFormTemplate> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new PrintFormTemplateDomainEntityDto
            {
                BranchOfficeOrganizationUnitRef = new EntityReference { Id = (parentEntityName == EntityType.Instance.BranchOfficeOrganizationUnit()) ? parentEntityId : null }
            };
        }
    }
}