using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> AdvertisementElementTemplateProperties =
            new[]
                {
                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(256),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.RestrictionType)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RestrictionType)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FileSizeRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileSizeRestriction)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FileNameLengthRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileNameLengthRestriction)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.TextLineBreaksCountRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TextLineBreaksCountRestriction)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.TextLengthRestriction)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TextLengthRestriction)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.MaxSymbolsInWord)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MaxSymbolsInWord)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.FormattedText)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FormattedText)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ImageDimensionRestriction)
                                  .WithFeatures(
                                      RegularExpressionPropertyFeature.Create(@"(\d+,\d+;[ ]?)+$", () => BLResources.FieldMustMatchImageDimensionRegex),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ImageDimensionRestriction)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsRequired)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsRequired)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsAdvertisementLink)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsAdvertisementLink)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementTemplateDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
