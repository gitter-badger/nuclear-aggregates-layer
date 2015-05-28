using System.Linq;

using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetThemeTemplateDtoService : GetDomainEntityDtoServiceBase<ThemeTemplate>
    {
        private readonly IFinder _finder;

        public GetThemeTemplateDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<ThemeTemplate> GetDto(long entityId)
        {
            var dto =
                _finder.FindObsolete(new FindSpecification<ThemeTemplate>(x => x.Id == entityId))
                       .Select(entity => new ThemeTemplateDomainEntityDto
                           {
                               Id = entity.Id,
                               FileId = entity.FileId,
                               TemplateCode = entity.TemplateCode,
                               IsSkyScraper = entity.IsSkyScraper,
                               IsTemplateUsedInThemes = entity.Themes.Any(y => y.IsActive && !y.IsDeleted),
                               FileContentType = entity.File.ContentType,
                               FileName = entity.File.FileName,
                               FileContentLength = entity.File.ContentLength,
                               CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                               CreatedOn = entity.CreatedOn,
                               IsActive = entity.IsActive,
                               IsDeleted = entity.IsDeleted,
                               ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                               ModifiedOn = entity.ModifiedOn,
                               Timestamp = entity.Timestamp
                           })
                       .Single();

            return dto;
        }

        protected override IDomainEntityDto<ThemeTemplate> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new ThemeTemplateDomainEntityDto();
        }
    }
}