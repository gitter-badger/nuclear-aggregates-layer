using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> AdsTemplatesAdsElementTemplateProperties =
            new[]
                {
                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.AdsTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.AdvertisementTemplate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdvertisementTemplateName)),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.AdsElementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.AdvertisementElementTemplate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdvertisementElementTemplateName)),


                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.ExportCode)
                                  .WithFeatures(
                                      RangePropertyFeature.Create(0, int.MaxValue, () => BLResources.ExportCodeRangeMessage),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ExportCode)),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),


                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdsTemplatesAdsElementTemplateDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
