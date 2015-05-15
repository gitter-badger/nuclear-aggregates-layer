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
        public static readonly IEnumerable<EntityPropertyMetadata> OrderPositionProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.OrderId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PricePositionRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePosition),
                                      LookupPropertyFeature.Create(EntityType.Instance.PricePosition())
                                                           .WithShowReadOnlyCard()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PricePerUnit)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(18),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePerUnit)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PricePerUnitWithVat)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(18),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePerUnitWithVat)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(8),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Quantity)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.DiscountSum)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountSum)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.DiscountPercent)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountPercent)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PayablePrice)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePrice)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PayablePlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePlan)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.ShipmentPlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ShipmentPlan)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      new ReadOnlyPropertyFeature(),
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.IsComposite)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.CalculateDiscountViaPercent),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PriceId),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.OrderFirmId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.OrderNumber),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.OrganizationUnitId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PeriodStartDate)
                                  .WithFeatures(new DatePropertyFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.PeriodEndDate)
                                  .WithFeatures(new DatePropertyFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.Advertisements)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    new EntityPropertyMetadata("PricePositionSearchFormFilter", typeof(string))
                        .WithFeatures(
                            new ReadOnlyPropertyFeature(),
                            new HiddenFeature()),

                    new EntityPropertyMetadata("IsLocked", typeof(bool))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
