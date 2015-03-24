using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> AccountDetailProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.AccountRef)
                                  /*.WithFeatures(
                                      new HiddenFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AccountId))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.OperationTypeRef)
                                  /*.WithFeatures(
                                      /*new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.OperationType())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationType))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.Description)
                                  /*.WithFeatures(
                                      /*DisplayNameLocalizedFeature.Create(() => MetadataResources.Description),
                                      new LimitedLengthPropertyFeature(200))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.TransactionDate)
                                  /*.WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TransactionDate),
                                      new DatePropertyFeature())*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.Amount)
                                  /*.WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Amount),
                                      RegularExpressionPropertyFeature.Create(@"^([\d]((.|,)[\d]{1,})+|[1-9][\d]*((.|,)[\d]{1,})?)$",
                                                                              () => BLResources.MustBePositiveDigit),
                                      new RequiredPropertyFeature())*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.OwnerCanBeChanged),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        /*.WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            })*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.Id)
                                  /*.WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.OwnerRef)
                                  /*.WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner))*/,


                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      //LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature()//,
                                      //new ReadOnlyPropertyFeature(),
                                      //DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)
                                      ),

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.CreatedOn)
                                  /*.WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.ModifiedByRef)
                                  /*.WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.ModifiedOn)
                                  /*.WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn))*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.IsActive)
                                  /*.WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.Timestamp)
                                  /*.WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())*/,

                    EntityPropertyMetadata.Create<AccountDetailDomainEntityDto>(dto => dto.IsDeleted)
                                  /*.WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())*/
                };
    }
}
