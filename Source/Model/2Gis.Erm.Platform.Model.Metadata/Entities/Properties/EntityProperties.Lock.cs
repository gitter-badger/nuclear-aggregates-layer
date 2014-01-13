using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> LockProperties =
            new[]
                {
                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.BranchOfficeOrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.BranchOfficeOrganizationUnit)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.OrderRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.Order)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrderNumber)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.LegalPerson)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreateDate)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.PeriodStartDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PeriodStartDate)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.PeriodEndDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(false, PeriodType.MonthlyUpperBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PeriodEndDate)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseLockDate)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.PlannedAmount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockPlannedAmount)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.Balance)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockBalance)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.ClosedBalance)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockClosedBalance)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.DebitAccountDetailRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.AccountDetail)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationOnAccount)),


                    new EntityProperty("Status", typeof(string))
                        .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Status)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LockDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
