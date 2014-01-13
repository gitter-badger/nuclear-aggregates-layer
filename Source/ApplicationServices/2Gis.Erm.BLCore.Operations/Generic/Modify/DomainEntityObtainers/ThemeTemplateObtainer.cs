﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Themes;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class ThemeTemplateObtainer : IBusinessModelEntityObtainer<ThemeTemplate>, IAggregateReadModel<Theme>
    {
        private readonly IFinder _finder;

        public ThemeTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public ThemeTemplate ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ThemeTemplateDomainEntityDto)domainEntityDto;

            var template = _finder.Find(ThemeSpecifications.Find.ThemeTemplateById(dto.Id)).SingleOrDefault() ??
                               new ThemeTemplate { IsActive = true };

            template.Id = dto.Id;
            template.TemplateCode = dto.TemplateCode;
            template.FileId = dto.FileId;
            template.IsSkyScraper = dto.IsSkyScraper;

            template.Timestamp = dto.Timestamp;

            return template;
        }
    }
}