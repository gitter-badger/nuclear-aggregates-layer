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
        public static readonly IEnumerable<EntityProperty> AccountDetailProperties =
            new[]
                {
                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.AccountRef)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AccountId)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.OperationTypeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.OperationType)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationType)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description),
                                      new LimitedLengthPropertyFeature(200)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.TransactionDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TransactionDate),
                                      new DatePropertyFeature()),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Amount),
                                      RegularExpressionPropertyFeature.Create(@"^([\d]((.|,)[\d]{1,})+|[1-9][\d]*((.|,)[\d]{1,})?)$",
                                                                              () => BLResources.MustBePositiveDigit),
                                      new RequiredPropertyFeature()),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.OwnerCanBeChanged),

                    new EntityProperty
                        {
                            Name = "IsSecurityRoot",
                            Type = typeof(bool),
                        }
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            }),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner)),


                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AccountDetailDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
