using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
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
        public static readonly IEnumerable<EntityPropertyMetadata> LimitProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.AccountRef)
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.BranchOfficeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.BranchOffice()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.LegalPerson()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalPerson)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.StartPeriodDate)
                                  .WithFeatures(new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.WithoutDayNumber),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Period)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(new DatePropertyFeature(false),
                                                new ReadOnlyPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.LimitCloseDate)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Amount)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.Status)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new ReadOnlyPropertyFeature(),
                                                new EnumPropertyFeature(BLResources.ResourceManager),
                                                new ExcludeZeroValuePropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Status)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.InspectorRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.User())
                                                           .WithExtendedInfo("privilege=" + (int)FunctionalPrivilegeName.LimitManagement + "&(userIdForOrgUnit={OwnerRef.Id})"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inspector)),

                    new EntityPropertyMetadata("HasEditPeriodPrivelege", typeof(bool))
                    .WithFeatures(new HiddenFeature()), 

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LimitDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
