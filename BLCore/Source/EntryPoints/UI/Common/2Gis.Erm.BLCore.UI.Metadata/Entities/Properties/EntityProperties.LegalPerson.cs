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
        public static readonly IEnumerable<EntityPropertyMetadata> LegalPersonProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.LegalName)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalName)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.ShortName)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ShortName)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.LegalPersonTypeEnum)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPersonType),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.IsInSyncWith1C)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(3),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.LegalAddress)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalAddress)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.Inn)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(10, 10),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.Kpp)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(9, 9),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Kpp)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.BusinessmanInn)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(12, 12),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.PassportSeries)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(4, 4),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportSeries)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.PassportNumber)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(6, 6),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportNumber)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.PassportIssuedBy)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(0, 512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PassportIssuedBy)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.RegistrationAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(0, 512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RegistrationAddress)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.Client()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ClientName)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.HasProfiles)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            })
                };
    }
}