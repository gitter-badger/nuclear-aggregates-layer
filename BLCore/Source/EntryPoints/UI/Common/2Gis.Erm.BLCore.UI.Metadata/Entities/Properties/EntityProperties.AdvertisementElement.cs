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
        public static readonly IEnumerable<EntityPropertyMetadata> AdvertisementElementProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.AdvertisementElementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdvertisementElementTemplateName),
                                      LookupPropertyFeature.Create(EntityType.Instance.AdvertisementElementTemplate())),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateFileExtensionRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateImageDimensionRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateRestrictionType)
                                  .WithFeatures(
                                  new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateTextLengthRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateMaxSymbolsInWord)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateTextLineBreaksRestriction)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateFormattedText)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.TemplateAdvertisementLink)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FormattedText)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.PlainText)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FasCommentType)
                                  .WithFeatures(
                                  new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FasComment)),

                    new EntityPropertyMetadata("FasCommentDisplayTextItems", typeof(IReadOnlyDictionary<string, string>))
                        .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.BeginDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginDate)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.EndDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndDate)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner)),


                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AdvertisementElementDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
