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
        public static readonly IEnumerable<EntityProperty> BranchOfficeOrganizationUnitProperties =
            new[]
                {
                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityName.BranchOffice),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeName)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityName.OrganizationUnit),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ShortLegalName)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ShortLegalName)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PositionInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInGenitive)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PositionInNominative)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInNominative)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ChiefNameInNominative)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInNominative)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ChiefNameInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInGenitive)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.OperatesOnTheBasisInGenitive)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.OperatesOnTheBasisInGenitive)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Kpp)
                                  .WithFeatures(new LimitedLengthPropertyFeature(9, 9),
                                                new DigitsOnlyPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Kpp)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PaymentEssentialElements)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(256),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PaymentEssentialElements)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ActualAddress)
                                  .WithFeatures(new LimitedLengthPropertyFeature(512),
                                                new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ActualAddress)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PostalAddress)
                                  .WithFeatures(new LimitedLengthPropertyFeature(512),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.PostalAddress)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.PhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PhoneNumber)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsPrimary)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsPrimary)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsPrimaryForRegionalSales)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsPrimaryForRegionalSales)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlId)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlName)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlLegalAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalAddress)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlInn)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.BranchOfficeAddlIc)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Ic)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.SyncCode1C)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SyncCode1C)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.RegistrationCertificate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RegistrationCertificate)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Email)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Email)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Id),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<BranchOfficeOrganizationUnitDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
