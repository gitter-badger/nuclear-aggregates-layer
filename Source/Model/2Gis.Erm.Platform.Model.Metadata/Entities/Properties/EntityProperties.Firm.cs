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
        public static readonly IEnumerable<EntityProperty> FirmProperties =
            new[]
                {
                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirmName)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ClosedForAscertainment)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsHiddenFemale)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.PromisingScore)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PromisingScore)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.UsingOtherMedia)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.UsingOtherMedia)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.MarketType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MarketType)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ProductType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ProductType)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.BudgetType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirmBudgetType)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.Geolocation)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Geolocation)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.InCityBranchesAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.InCityBranchesAmount)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.OutCityBranchesAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OutCityBranchesAmount)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.StaffAmount)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.StaffAmount)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.LastQualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastQualifyTime)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.LastDisqualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastDisqualifyTime)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Client),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Client)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ClientReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.TerritoryRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Territory),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Territory)),


                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.OrganizationUnit),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityProperty("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityProperty.Create<FirmDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
