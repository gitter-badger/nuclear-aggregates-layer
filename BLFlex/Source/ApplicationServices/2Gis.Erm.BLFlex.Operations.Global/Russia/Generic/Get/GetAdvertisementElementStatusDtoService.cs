using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings.Queryable;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetAdvertisementElementStatusDtoService : GetDomainEntityDtoServiceBase<AdvertisementElementStatus>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;

        public GetAdvertisementElementStatusDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AdvertisementElementStatus> GetDto(long entityId)
        {
            var advertisementElementStatusDomainEntityDto = _finder.FindObsolete(Specs.Find.ById<AdvertisementElementStatus>(entityId))
                                                                   .Select(entity => new AdvertisementElementStatusDomainEntityDto
                                                                       {
                                                                           Id = entity.Id,
                                                                           Status = entity.Status,
                                                                           CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                                           CreatedOn = entity.CreatedOn,
                                                                           ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                                           ModifiedOn = entity.ModifiedOn,
                                                                           Timestamp = entity.Timestamp,
                                                                       })
                                                                   .Single();

            advertisementElementStatusDomainEntityDto.Reasons = _finder.Find(Specs.Find.ById<AdvertisementElement>(entityId))
                                                                       .Map(q => q.SelectMany(entity => entity.AdvertisementElementDenialReasons)
                                                                                  .Select(reason => new ReasonDto
                                                                                      {
                                                                                          Id = reason.Id,
                                                                                          Comment = reason.Comment,
                                                                                      }))
                                                                       .Many()
                                                                       .ToArray();

            var advertisementElementDomainEntityDto =
                _finder.FindObsolete(Specs.Find.ById<AdvertisementElement>(entityId))
                       .Select(entity => new AdvertisementElementDomainEntityDto
                           {
                               TemplateRestrictionType = entity.AdvertisementElementTemplate.RestrictionType,
                               TemplateFileExtensionRestriction = entity.AdvertisementElementTemplate.FileExtensionRestriction,
                               TemplateImageDimensionRestriction = entity.AdvertisementElementTemplate.ImageDimensionRestriction,
                               TemplateTextLengthRestriction = entity.AdvertisementElementTemplate.TextLengthRestriction,
                               TemplateMaxSymbolsInWord = entity.AdvertisementElementTemplate.MaxSymbolsInWord,
                               TemplateTextLineBreaksRestriction = entity.AdvertisementElementTemplate.TextLineBreaksCountRestriction,
                               TemplateAdvertisementLink = entity.AdvertisementElementTemplate.IsAdvertisementLink,
                               TemplateFormattedText = entity.AdvertisementElementTemplate.FormattedText,

                               PlainText = entity.Text,
                               FormattedText = entity.Text,

                               BeginDate = entity.BeginDate,
                               EndDate = entity.EndDate,

                               FasCommentType = (FasComment)entity.FasCommentType,

                               FileId = entity.FileId,
                               FileName = entity.File != null ? entity.File.FileName : null,
                               FileContentLength = entity.File != null ? entity.File.ContentLength : 0,
                               FileContentType = entity.File != null ? entity.File.ContentType : null,
                               Timestamp = entity.Timestamp
                           })
                       .Single();

            advertisementElementDomainEntityDto.TransferRestrictionValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferTextValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferFileValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferPeriodValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferFasCommentValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferLinkValuesTo(advertisementElementStatusDomainEntityDto);
            advertisementElementDomainEntityDto.TransferTimestampTo(advertisementElementStatusDomainEntityDto);

            return advertisementElementStatusDomainEntityDto;
        }

        protected override IDomainEntityDto<AdvertisementElementStatus> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            throw new NotSupportedException();
        }
    }
}