using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> PriceProperties =
            new[]
                {
                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.CreateDate)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceCreateDate)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.PublishDate)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, DateTime.Today),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePublishDate)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.BeginDate)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.Full, DateTime.Now.AddDays(1)),
                                      MustBeGreaterOrEqualPropertyFeature.Create<PriceDomainEntityDto>(dto => dto.PublishDate,
                                                                                                       () =>
                                                                                                       BLResources.BeginDateMustBeGreaterOrEqualThenPublishDate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PublishBeginDate)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.OrganizationUnit)
                                                           .WithSearchFormFilterInfo("IsDeleted=false&&IsActive=true"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.IsPublished)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceIsPublished)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<PriceDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
