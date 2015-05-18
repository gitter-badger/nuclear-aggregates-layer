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
        public static readonly IEnumerable<EntityPropertyMetadata> AccountProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AccountDomainEntityDto>(dto => dto.BranchOfficeOrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.BranchOfficeOrganizationUnit()),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityPropertyMetadata.Create<AccountDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.LegalPerson()),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),
                                      /*
                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Currency()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Currency)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.AccountDetailBalance)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AccountDetailBalance)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.LockDetailBalance)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockDetailBalance)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.LegalPesonSyncCode1C)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPesonSyncCode1C)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.OwnerCanBeChanged),

                    new EntityProperty("IsSecurityRoot", typeof(bool))
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            }),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())*/
                };
    }
}
