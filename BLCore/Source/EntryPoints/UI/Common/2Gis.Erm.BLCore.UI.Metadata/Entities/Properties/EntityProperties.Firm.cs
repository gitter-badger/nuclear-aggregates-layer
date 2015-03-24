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
        public static readonly IEnumerable<EntityPropertyMetadata> FirmProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirmName)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ClosedForAscertainment)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsHiddenFemale)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.PromisingScore)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PromisingScore)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.UsingOtherMedia)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.UsingOtherMedia)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.MarketType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MarketType)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ProductType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ProductType)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.BudgetType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirmBudgetType)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.Geolocation)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Geolocation)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.InCityBranchesAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.InCityBranchesAmount)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.OutCityBranchesAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OutCityBranchesAmount)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.StaffAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.StaffAmount)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.LastQualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastQualifyTime)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.LastDisqualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastDisqualifyTime)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Client()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Client)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ClientReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.TerritoryRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Territory()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Territory)),


                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.OrganizationUnit()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
