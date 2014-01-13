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
        public static readonly IEnumerable<EntityProperty> PlatformProperties =
            new[]
                {
                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.Id),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.PlacementPeriodEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PlacementPeriod)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.MinPlacementPeriodEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MinPlacementPeriod)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.DgppId)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DgppId)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.IsSupportedByExport)
                                  .WithFeatures(
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsSupportedByExport)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PlatformDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
