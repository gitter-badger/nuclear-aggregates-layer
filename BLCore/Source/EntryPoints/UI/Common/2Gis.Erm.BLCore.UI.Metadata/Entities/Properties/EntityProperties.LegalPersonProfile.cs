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
        public static readonly IEnumerable<EntityPropertyMetadata> LegalPersonProfileProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.LegalPersonType)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.PositionInGenitive)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInGenitive)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.PositionInNominative)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionInNominative)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.ChiefNameInNominative)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInNominative)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.ChiefNameInGenitive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ChiefNameInGenitive)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.OperatesOnTheBasisInGenitive)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperatesOnTheBasisInGenitive),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.DocumentsDeliveryAddress)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DocumentsDeliveryAddress)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.CertificateNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CertificateNumber)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.CertificateDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CertificateDate),
                                      new DatePropertyFeature(false)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.WarrantyNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WarrantyNumber)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.WarrantyBeginDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WarrantyBeginDate),
                                      new DatePropertyFeature(false)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.WarrantyEndDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WarrantyEndDate),
                                      new DatePropertyFeature(false)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.BargainNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainNumber)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.BargainBeginDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainBeginDate),
                                      new DatePropertyFeature(false)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.BargainEndDate)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainEndDate),
                                      new DatePropertyFeature(false)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.PostAddress)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PostAddress)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.RecipientName)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.RecipientName)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.DocumentsDeliveryMethod)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DocumentsDeliveryMethod),
                                      new EnumPropertyFeature(EnumResources.ResourceManager)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.EmailForAccountingDocuments)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EmailForAccountingDocuments)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.Email)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalEmail)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.PersonResponsibleForDocuments)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PersonResponsibleForDocuments)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.Phone)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(50),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ContactPhone)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.PaymentEssentialElements)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PaymentEssentialElements)),


                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.LegalPersonRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.LegalPerson)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.IsMainProfile)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new ReadOnlyPropertyFeature()),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            }),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.OwnerRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Owner)),


                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<LegalPersonProfileDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
