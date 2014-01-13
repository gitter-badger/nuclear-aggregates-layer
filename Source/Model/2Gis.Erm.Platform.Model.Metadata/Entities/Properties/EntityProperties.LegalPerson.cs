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
        public static readonly IEnumerable<EntityProperty> LegalPersonProperties =
            new[]
                {
                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.LegalName)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalName)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.ShortName)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ShortName)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.LegalPersonTypeEnum)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPersonType),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.IsInSyncWith1C)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(3),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.LegalAddress)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalAddress)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.Inn)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(10, 10),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.Kpp)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(9, 9),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Kpp)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.BusinessmanInn)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(12, 12),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.PassportSeries)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(4, 4),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportSeries)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.PassportNumber)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(6, 6),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportNumber)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.PassportIssuedBy)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(0, 512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportIssuedBy)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.RegistrationAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(0, 512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RegistrationAddress)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.Client),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ClientName)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.HasProfiles)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LegalPersonDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityProperty("IsSecurityRoot", typeof(bool))
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            })
                };
    }
}