using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> AdvertisementElementProperties =
            new[]
                {
                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.AdvertisementElementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdvertisementElementTemplateName),
                                      LookupPropertyFeature.Create(EntityName.AdvertisementElementTemplate)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateFileExtensionRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateImageDimensionRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateRestrictionType)
                                  .WithFeatures(
                                  new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateTextLengthRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateMaxSymbolsInWord)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateTextLineBreaksRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateFormattedText)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateAdvertisementLink)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FormattedText)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.PlainText)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FasCommentType)
                                  .WithFeatures(
                                  new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FasComment)),

                    new EntityProperty("FasCommentDisplayText", typeof(FasCommentDisplayText))
                        .WithFeatures(new HiddenFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.BeginDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginDate)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.EndDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndDate)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileTimestamp)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner)),


                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementElementDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
