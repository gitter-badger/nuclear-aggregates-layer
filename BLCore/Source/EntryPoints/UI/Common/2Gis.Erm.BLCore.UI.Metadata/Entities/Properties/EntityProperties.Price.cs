using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> PriceProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.CreateDate)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceCreateDate)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.PublishDate)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, DateTime.Today),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePublishDate)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.BeginDate)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.Full, DateTime.Now.AddDays(1)),
                                      MustBeGreaterOrEqualPropertyFeature.Create<PriceDomainEntityDto>(dto => dto.PublishDate,
                                                                                                       () =>
                                                                                                       BLResources.BeginDateMustBeGreaterOrEqualThenPublishDate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PublishBeginDate)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.IsPublished)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceIsPublished)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PriceDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
