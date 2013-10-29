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
        public static readonly IEnumerable<EntityProperty> OrderPositionProperties =
            new[]
                {
                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.OrderId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PricePositionRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePosition),
                                      LookupPropertyFeature.Create(EntityName.PricePosition)
                                                           .WithShowReadOnlyCard()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PricePerUnit)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(18),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePerUnit)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PricePerUnitWithVat)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(18),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePerUnitWithVat)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(8),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Quantity)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.DiscountSum)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountSum)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.DiscountPercent)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountPercent)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PayablePrice)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePrice)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PayablePlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePlan)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.ShipmentPlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ShipmentPlan)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      new ReadOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.IsComposite)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.CalculateDiscountViaPercent),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PriceId),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.OrderFirmId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.OrderNumber),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.OrganizationUnitId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PeriodStartDate)
                                  .WithFeatures(new DatePropertyFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.PeriodEndDate)
                                  .WithFeatures(new DatePropertyFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.Advertisements)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    new EntityProperty("PricePositionSearchFormFilter", typeof(string))
                        .WithFeatures(
                            new ReadOnlyPropertyFeature(),
                            new HiddenFeature()),

                    new EntityProperty("IsLocked", typeof(bool))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<OrderPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
