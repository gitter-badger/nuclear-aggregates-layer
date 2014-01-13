﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdsTemplatesAdsElementTemplateObtainer : IBusinessModelEntityObtainer<AdsTemplatesAdsElementTemplate>, IAggregateReadModel<Advertisement>
    {
        private IFinder _finder;

        public AdsTemplatesAdsElementTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdsTemplatesAdsElementTemplate ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdsTemplatesAdsElementTemplateDomainEntityDto)domainEntityDto;

            var entity =
                dto.Id == 0
                    ? new AdsTemplatesAdsElementTemplate()
                    : _finder.Find(AdvertisementSpecifications.Find.AdsTemplatesAdsElementTemplateById(dto.Id)).Single();

            entity.AdsTemplateId = dto.AdsTemplateRef.Id.Value;
            entity.AdsElementTemplateId = dto.AdsElementTemplateRef.Id.Value;
            entity.ExportCode = dto.ExportCode;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}