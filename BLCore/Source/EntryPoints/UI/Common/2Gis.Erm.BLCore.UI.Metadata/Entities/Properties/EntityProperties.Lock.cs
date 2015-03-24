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
        public static readonly IEnumerable<EntityPropertyMetadata> LockProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.BranchOfficeOrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.BranchOfficeOrganizationUnit())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.OrderRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.Order())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrderNumber)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.LegalPerson())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreateDate)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.PeriodStartDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PeriodStartDate)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.PeriodEndDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false, PeriodType.MonthlyUpperBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PeriodEndDate)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseLockDate)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.PlannedAmount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockPlannedAmount)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.Balance)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockBalance)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.ClosedBalance)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockClosedBalance)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.DebitAccountDetailRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.AccountDetail())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationOnAccount)),


                    new EntityPropertyMetadata("Status", typeof(string))
                        .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Status)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LockDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
