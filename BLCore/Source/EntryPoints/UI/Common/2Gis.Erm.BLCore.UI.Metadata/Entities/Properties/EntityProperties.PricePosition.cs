using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> PricePositionProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.PositionRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Position())
                                                           .WithExtendedInfo("isSupportedByExport=true"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.Cost)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Cost)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Currency)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.PriceRef)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceList)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.AmountSpecificationMode)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Quantity)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Quantity)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.MinAdvertisementAmount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MaxAdvertisementAmount)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.RateType)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RateType)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.IsRateTypeAvailable)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.IsPositionControlledByAmount)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PricePositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
