using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> OrderProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.Number)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(200),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrderNumber)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.RegionalNumber)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(200),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RegionalNumber)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.FirmRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.Firm)
                                                           .WithExtendedInfo("organizationUnitId={DestOrganizationUnitRef.Id}&clientId={ClientRef.Id}"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Firm)),
                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.HasAnyOrderPosition)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.HasDestOrganizationUnitPublishedPrice)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.SourceOrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.OrganizationUnit)
                                                           .WithShowReadOnlyCard()
                                                           .WithExtendedInfo("currencyId={DealCurrencyId}&userId={CurrenctUserCode}"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SourceOrganizationUnit)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DestOrganizationUnitRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.OrganizationUnit)
                                                           .WithShowReadOnlyCard()
                                                           .WithExtendedInfo("restrictByProjects=true"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SourceOrganizationUnit)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.BranchOfficeOrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.BranchOfficeOrganizationUnit)
                                                           .WithExtendedInfo("OrderSourceOrganizationUnitId={SourceOrganizationUnitRef.Id}"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnitName)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.LegalPerson)
                                                           .WithExtendedInfo("ClientId={ClientRef.Id}")
                                                           .OverrideValueAttribute<LegalPersonDomainEntityDto>(x => x.LegalName),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DealRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Deal),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Deal)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DealCurrencyId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.BeginDistributionDate)
                                  .WithFeatures(
                                      CheckDatePropertyFeature.Create(CheckDayOfMonthType.FirstDay, () => BLResources.RequiredFirstDayOfMonthMessage),
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginReleaseDate),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.EndDistributionDatePlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      CheckDatePropertyFeature.Create(CheckDayOfMonthType.LastDay, () => BLResources.RequiredLastDayOfMonthMessage),
                                      MustBeGreaterOrEqualPropertyFeature.Create<OrderDomainEntityDto>(dto => dto.BeginDistributionDate,
                                                                                                       () => BLResources.IncorrectBeginDistributionDate),
                                      new DatePropertyFeature(false, PeriodType.MonthlyUpperBound, DisplayStyle.Full),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndPlanReleaseDate),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.EndDistributionDateFact)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndFactReleaseDate),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.SignupDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SignupDate),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.BeginReleaseNumber)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginReleaseNumber),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.EndReleaseNumberPlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndPlanReleaseNumber),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.EndReleaseNumberFact)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndFactReleaseNumber),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ReleaseCountPlan)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      RangePropertyFeature.Create(1, 12, () => BLResources.ReleaseCountPlanRangeMessage),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PlanReleaseCount),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ReleaseCountFact)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FactReleaseCount),
                                      GroupedPropertyFeature.Create(() => BLResources.TitlePlacement)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Currency)
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Currency),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.PreviousWorkflowStepId)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.WorkflowStepId)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DgppId)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DgppId)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.PayablePrice)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePrice),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.PayablePlan)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayablePlan),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.PayableFact)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PayableFact),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.VatPlan)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.AmountToWithdraw)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AmountToWithdraw),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.AmountWithdrawn)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AmountWithdrawn),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleFinances)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.BargainRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Bargain)
                                                           .OverrideValueAttribute<BargainDomainEntityDto>(x => x.Number),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Bargain)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DiscountSum)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountSum),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleDiscount)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DiscountPercent)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountPercent),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleDiscount)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DiscountPercentChecked)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleDiscount)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DiscountReasonEnum)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountReason),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleDiscount)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DiscountComment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DiscountComment),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleDiscount)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CancellationComment),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleCancellation)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.IsTerminated)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsTerminated),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.TerminationReason)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TerminationReason),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleCancellation),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.OrderType)
                                  .WithFeatures(
                                      new ExcludeZeroValuePropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrderType)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.InspectorRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User)
                                                           .WithShowReadOnlyCard()
                                                           .WithExtendedInfo("&orgUnitId={SourceOrganizationUnitRef.Id}"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inspector),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleControl)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.Platform)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.PlatformRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.HasDocumentsDebt)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.HasDocumentsDebt),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleControl)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.DocumentsComment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DocumentsComment),
                                      GroupedPropertyFeature.Create(() => BLResources.TitleControl)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.AccountRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ShowRegionalAttributes)
                                  .WithFeatures(new HiddenFeature()),

                    // TODO: далее идет ряд признаков наличия привилегий. 
                    // Можно использовать сервис метаданных для получения информации о этих привелегиях 
                    new EntityPropertyMetadata("HasOrderCreationExtended", typeof(bool))
                        .WithFeatures(
                            new ReadOnlyPropertyFeature(),
                            new HiddenFeature()),

                    new EntityPropertyMetadata("CanEditOrderType", typeof(bool))
                        .WithFeatures(
                            new ReadOnlyPropertyFeature(),
                            new HiddenFeature()),

                    new EntityPropertyMetadata("CurrenctUserCode", typeof(long))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    new EntityPropertyMetadata("MakeReadOnly", typeof(bool))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    new EntityPropertyMetadata("IsWorkflowLocked", typeof(bool))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    new EntityPropertyMetadata("AvailableSteps", typeof(string))
                        .WithFeatures(
                            new HiddenFeature(),
                            new ReadOnlyPropertyFeature()),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn),
                                      GroupedPropertyFeature.Create(() => BLResources.AdministrationTabTitle)),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
