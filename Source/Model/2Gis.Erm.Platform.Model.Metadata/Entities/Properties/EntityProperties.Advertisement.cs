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
        public static readonly IEnumerable<EntityProperty> AdvertisementProperties =
            new[]
                {
                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.HasAssignedOrder),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(128),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(2),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.AdvertisementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.AdvertisementTemplate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdvertisementTemplate)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.FirmRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Firm),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Firm)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.IsSelectedToWhiteList)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => BLResources.AdvertisementIsSelectedToWhiteList)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AdvertisementDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
