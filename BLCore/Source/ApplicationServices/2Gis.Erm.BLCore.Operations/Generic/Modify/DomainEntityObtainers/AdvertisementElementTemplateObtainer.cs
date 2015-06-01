using System.Drawing.Imaging;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdvertisementElementTemplateObtainer : IBusinessModelEntityObtainer<AdvertisementElementTemplate>, IAggregateReadModel<Advertisement>
    {
        private readonly IFinder _finder;

        public AdvertisementElementTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdvertisementElementTemplate ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementTemplateDomainEntityDto)domainEntityDto;

            // todo: добавить контроль сохранения удалённой сущности
            var entity = _finder.Find(Specs.Find.ById<AdvertisementElementTemplate>(dto.Id)).One() ??
                new AdvertisementElementTemplate { Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Name = dto.Name;
            entity.RestrictionType = dto.RestrictionType;
            entity.FileNameLengthRestriction = dto.FileNameLengthRestriction;
            entity.FileSizeRestriction = dto.FileSizeRestriction;
            entity.ImageDimensionRestriction = dto.ImageDimensionRestriction;
            entity.FormattedText = dto.FormattedText;
            entity.TextLengthRestriction = dto.TextLengthRestriction;
            entity.MaxSymbolsInWord = dto.MaxSymbolsInWord;
            entity.TextLineBreaksCountRestriction = dto.TextLineBreaksCountRestriction;
            entity.IsRequired = dto.IsRequired;
            entity.IsAdvertisementLink = dto.IsAdvertisementLink;
            entity.Timestamp = dto.Timestamp;
            entity.NeedsValidation = dto.NeedsValidation;

            var restrictions = new[]
            {
                dto.IsPngSupported ? ImageFormat.Png.ToString().ToLowerInvariant() : null,
                dto.IsGifSupported ? ImageFormat.Gif.ToString().ToLowerInvariant() : null,
                dto.IsBmpSupported ? ImageFormat.Bmp.ToString().ToLowerInvariant() : null,
                dto.IsChmSupported ? "chm" : null
            }
            .Where(x => x != null);

            entity.FileExtensionRestriction = string.Join(" | ", restrictions);

            return entity;
        }
    }
}