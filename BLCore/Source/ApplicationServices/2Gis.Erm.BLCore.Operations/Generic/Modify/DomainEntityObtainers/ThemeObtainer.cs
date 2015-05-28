using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class ThemeObtainer : IBusinessModelEntityObtainer<Theme>, IAggregateReadModel<Theme>
    {
        private readonly IFinder _finder;

        public ThemeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Theme ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ThemeDomainEntityDto)domainEntityDto;

            var theme = _finder.Find(Specs.Find.ById<Theme>(dto.Id)).One() ??
                new Theme { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && theme.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            theme.Name = dto.Name;
            theme.Description = dto.Description;
            theme.ThemeTemplateId = dto.ThemeTemplateRef.Id.Value;
            theme.BeginDistribution = dto.BeginDistribution;
            theme.EndDistribution = dto.EndDistribution.GetEndPeriodOfThisMonth();
            theme.IsDefault = dto.IsDefault;
            theme.FileId = dto.FileId;

            return theme;
        }
    }
}