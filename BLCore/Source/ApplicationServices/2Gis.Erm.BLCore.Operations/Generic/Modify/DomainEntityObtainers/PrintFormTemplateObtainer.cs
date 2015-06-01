using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PrintFormTemplateObtainer : ISimplifiedModelEntityObtainer<PrintFormTemplate>
    {
        private readonly IFinder _finder;

        public PrintFormTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public PrintFormTemplate ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PrintFormTemplateDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<PrintFormTemplate>(dto.Id)).One()
                ?? new PrintFormTemplate { IsActive = true };

            entity.TemplateCode = dto.TemplateCode;
            entity.FileId = dto.FileId;
            entity.BranchOfficeOrganizationUnitId = dto.BranchOfficeOrganizationUnitRef.Id;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}