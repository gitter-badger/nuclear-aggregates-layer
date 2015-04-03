using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> AdvertisementElementTemplateProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(256),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.RestrictionType)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RestrictionType)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FileSizeRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileSizeRestriction)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FileNameLengthRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileNameLengthRestriction)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.TextLineBreaksCountRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TextLineBreaksCountRestriction)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.TextLengthRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TextLengthRestriction)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.MaxSymbolsInWord)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MaxSymbolsInWord)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FormattedText)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FormattedText)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ImageDimensionRestriction)
                                  .WithFeatures(
                                      RegularExpressionPropertyFeature.Create(@"(\d+,\d+;[ ]?)+$", () => BLResources.FieldMustMatchImageDimensionRegex),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ImageDimensionRestriction)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsRequired)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsRequired)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsAdvertisementLink)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsAdvertisementLink)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
