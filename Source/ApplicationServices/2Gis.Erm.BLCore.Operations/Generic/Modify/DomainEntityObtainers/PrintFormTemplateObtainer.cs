using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

            var entity = dto.Id == 0
                             ? new PrintFormTemplate { IsActive = true }
                             : _finder.Find(PrintFormTemplateSpecifications.Find.ById(dto.Id)).Single();

            entity.TemplateCode = (int)dto.TemplateCode;
            entity.FileId = dto.FileId;
            entity.BranchOfficeOrganizationUnitId = dto.BranchOfficeOrganizationUnitRef.Id;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}