using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetThemeDtoService : GetDomainEntityDtoServiceBase<Theme>
    {
        private readonly IFinder _finder;
        private readonly IThemeRepository _themeRepository;

        public GetThemeDtoService(IUserContext userContext, IFinder finder, IThemeRepository themeRepository) : base(userContext)
        {
            _finder = finder;
            _themeRepository = themeRepository;
        }

        protected override IDomainEntityDto<Theme> GetDto(long entityId)
        {
            var dto =
                _finder.FindObsolete(new FindSpecification<Theme>(x => x.Id == entityId))
                       .Select(entity => new ThemeDomainEntityDto
                           {
                               Id = entity.Id,
                               Name = entity.Name,
                               Description = entity.Description,
                               BeginDistribution = entity.BeginDistribution,
                               EndDistribution = entity.EndDistribution,
                               IsDefault = entity.IsDefault,
                               FileId = entity.FileId,
                               FileContentType = entity.File.ContentType,
                               FileName = entity.File.FileName,
                               FileContentLength = entity.File.ContentLength,
                               ThemeTemplateRef = new EntityReference { Id = entity.ThemeTemplateId },
                               ThemeTemplateCode = (ThemeTemplateCode)entity.ThemeTemplate.TemplateCode,
                               CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                               CreatedOn = entity.CreatedOn,
                               IsActive = entity.IsActive,
                               IsDeleted = entity.IsDeleted,
                               ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                               ModifiedOn = entity.ModifiedOn,
                               Timestamp = entity.Timestamp,
                           })
                       .Single();

            dto.OrganizationUnitCount = _themeRepository.CountThemeOrganizationUnits(entityId);

            return dto;
        }

        protected override IDomainEntityDto<Theme> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new ThemeDomainEntityDto
                {
                    BeginDistribution = DateTime.Now.GetNextMonthFirstDate(),
                    EndDistribution = DateTime.Now.GetNextMonthLastDate(),
                    OrganizationUnitCount = 0,
                };

            return dto;
        }
    }
}