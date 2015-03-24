using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> BillProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.Number)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BillNumber)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.BillDate)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BillDate)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.BeginDistributionDate)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginDistributionDate)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.EndDistributionDate)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndDistributionDate)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.PaymentDatePlan)
                                  .WithFeatures(
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PaymentDatePlan)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.PayablePlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePlan)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.IsOrderActive)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BillDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
