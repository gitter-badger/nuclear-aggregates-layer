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
        public static readonly IEnumerable<EntityPropertyMetadata> CurrencyProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(128),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.ISOCode)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CurrencyIsoCode)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.Symbol)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(10),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CurrencySymbol)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.IsBase)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsBaseCurrency)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<CurrencyDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
