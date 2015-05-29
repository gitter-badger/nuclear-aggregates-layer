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
        public static readonly IEnumerable<EntityPropertyMetadata> OrganizationUnitProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.Code)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Code)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.CountryRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Country()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CountryName)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.FirstEmitDate)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirstEmitDate)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.ErmLaunchDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ErmLaunchDate)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.InfoRussiaLaunchDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.InfoRussiaLaunchDate)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.TimeZoneRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.TimeZone()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TimeZoneId)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.ElectronicMedia)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ElectronicMedia)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.SyncCode1C)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SyncCode1C)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrganizationUnitDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
