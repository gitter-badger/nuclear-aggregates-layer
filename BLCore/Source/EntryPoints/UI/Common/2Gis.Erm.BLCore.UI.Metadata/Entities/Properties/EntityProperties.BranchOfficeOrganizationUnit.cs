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
        public static readonly IEnumerable<EntityPropertyMetadata> BranchOfficeOrganizationUnitProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityType.Instance.BranchOffice()),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeName)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityType.Instance.OrganizationUnit()),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ShortLegalName)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ShortLegalName)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PositionInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInGenitive)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PositionInNominative)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInNominative)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ChiefNameInNominative)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInNominative)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ChiefNameInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInGenitive)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.OperatesOnTheBasisInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.OperatesOnTheBasisInGenitive)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Kpp)
                                  .WithFeatures(new LimitedLengthPropertyFeature(9, 9),
                                                new DigitsOnlyPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Kpp)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PaymentEssentialElements)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(256),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PaymentEssentialElements)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ActualAddress)
                                  .WithFeatures(new LimitedLengthPropertyFeature(512),
                                                new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ActualAddress)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PostalAddress)
                                  .WithFeatures(new LimitedLengthPropertyFeature(512),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PostalAddress)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PhoneNumber)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsPrimary)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsPrimary)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsPrimaryForRegionalSales)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsPrimaryForRegionalSales)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlName)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlLegalAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalAddress)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlInn)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlIc)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Ic)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.SyncCode1C)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SyncCode1C)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.RegistrationCertificate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RegistrationCertificate)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Email)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Email)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
