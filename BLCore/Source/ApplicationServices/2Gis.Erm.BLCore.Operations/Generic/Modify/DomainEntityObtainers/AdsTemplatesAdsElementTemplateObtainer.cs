using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdsTemplatesAdsElementTemplateObtainer : IBusinessModelEntityObtainer<AdsTemplatesAdsElementTemplate>, IAggregateReadModel<Advertisement>
    {
        private readonly IFinder _finder;

        public AdsTemplatesAdsElementTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdsTemplatesAdsElementTemplate ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdsTemplatesAdsElementTemplateDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<AdsTemplatesAdsElementTemplate>(dto.Id)) 
                ?? new AdsTemplatesAdsElementTemplate();

            entity.AdsTemplateId = dto.AdsTemplateRef.Id.Value;
            entity.AdsElementTemplateId = dto.AdsElementTemplateRef.Id.Value;
            entity.ExportCode = dto.ExportCode;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}