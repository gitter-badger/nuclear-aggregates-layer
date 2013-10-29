using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> LimitProperties =
            new[]
                {
                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.AccountRef)
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.BranchOfficeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.BranchOffice),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.LegalPerson),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.StartPeriodDate)
                                  .WithFeatures(new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.WithoutDayNumber),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Period)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(new DatePropertyFeature(false),
                                                new ReadOnlyPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.LimitCloseDate)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Amount)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.Status)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new ReadOnlyPropertyFeature(),
                                                new EnumPropertyFeature(BLResources.ResourceManager),
                                                new ExcludeZeroValuePropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Status)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.InspectorRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.User)
                                                           .WithExtendedInfo("privilege=" + (int)FunctionalPrivilegeName.LimitManagement + "&(userIdForOrgUnit={OwnerRef.Id})"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inspector)),

                    new EntityProperty("HasEditPeriodPrivelege", typeof(bool))
                    .WithFeatures(new HiddenFeature()), 

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LimitDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
