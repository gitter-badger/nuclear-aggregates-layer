﻿using System.Drawing.Imaging;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementElementTemplateDtoService : GetDomainEntityDtoServiceBase<AdvertisementElementTemplate>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetAdvertisementElementTemplateDtoService(IUserContext userContext, 
            ISecureFinder finder, 
            IAPIIdentityServiceSettings identityIdentityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityIdentityServiceSettings;
        }

        protected override IDomainEntityDto<AdvertisementElementTemplate> GetDto(long entityId)
        {
            var png = ImageFormat.Png.ToString().ToLowerInvariant();
            var gif = ImageFormat.Gif.ToString().ToLowerInvariant();
            var bmp = ImageFormat.Bmp.ToString().ToLowerInvariant();
            const string chm = "chm";

            var dto = (from template in _finder.Find<AdvertisementElementTemplate>(x => x.Id == entityId)
                       let dummyAdsElementId = template.AdvertisementElements
                                                       .Where(y => y.Advertisement.FirmId == null && !y.IsDeleted && !y.Advertisement.IsDeleted)
                                                       .Select(y => y.Id)
                                                       .FirstOrDefault()
                       select new AdvertisementElementTemplateDomainEntityDto
                           {
                               Name = template.Name,
                               Id = template.Id,
                               FileNameLengthRestriction = template.FileNameLengthRestriction,
                               FileSizeRestriction = template.FileSizeRestriction,
                               ImageDimensionRestriction = template.ImageDimensionRestriction,
                               IsRequired = template.IsRequired,
                               FormattedText = template.FormattedText,
                               IsAdvertisementLink = template.IsAdvertisementLink,
                               RestrictionType = template.RestrictionType,
                               TextLengthRestriction = template.TextLengthRestriction,
                               MaxSymbolsInWord = template.MaxSymbolsInWord,
                               TextLineBreaksCountRestriction = template.TextLineBreaksCountRestriction,
                               Timestamp = template.Timestamp,
                               CreatedByRef = new EntityReference { Id = template.CreatedBy, Name = null },
                               CreatedOn = template.CreatedOn,
                               IsDeleted = template.IsDeleted,
                               NeedsValidation = template.NeedsValidation,
                               ModifiedByRef = new EntityReference { Id = template.ModifiedBy, Name = null },
                               ModifiedOn = template.ModifiedOn,
                               DummyAdvertisementElementRef = new EntityReference
                                {
                                    Id = dummyAdsElementId,
                                    Name = MetadataResources.DummyAdvertisement,
                                },
                               IsPngSupported = template.FileExtensionRestriction != null && template.FileExtensionRestriction.Contains(png),
                               IsGifSupported = template.FileExtensionRestriction != null && template.FileExtensionRestriction.Contains(gif),
                               IsBmpSupported = template.FileExtensionRestriction != null && template.FileExtensionRestriction.Contains(bmp),
                               IsChmSupported = template.FileExtensionRestriction != null && template.FileExtensionRestriction.Contains(chm),
                           }).Single();

            return dto;
        }

        protected override IDomainEntityDto<AdvertisementElementTemplate> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new AdvertisementElementTemplateDomainEntityDto
            {
                IdentityServiceUrl = _identityServiceSettings.RestUrl
            };
        }
    }
}