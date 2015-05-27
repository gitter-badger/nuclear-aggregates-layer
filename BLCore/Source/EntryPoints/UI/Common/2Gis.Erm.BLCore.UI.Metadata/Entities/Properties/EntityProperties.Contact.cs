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
        public static readonly IEnumerable<EntityPropertyMetadata> ContactProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(new PresentationLayerPropertyFeature(),
                                                new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.FullName)
                                  .WithFeatures(new LimitedLengthPropertyFeature(160),
                                                new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.FirstName)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FirstName),
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(160)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.MiddleName)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MiddleName),
                                      new LimitedLengthPropertyFeature(160)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.LastName)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastName),
                                      new LimitedLengthPropertyFeature(160)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Salutation)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ComboBoxPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Salutation),
                                      new LimitedLengthPropertyFeature(160)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.MainPhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WorkPhoneNumber),
                                      new LimitedLengthPropertyFeature(64)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.AdditionalPhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalPhoneNumber),
                                      new LimitedLengthPropertyFeature(64)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.MobilePhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MobilePhoneNumber),
                                      new LimitedLengthPropertyFeature(64)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.HomePhoneNumber)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.HomePhoneNumber),
                                      new LimitedLengthPropertyFeature(64)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Fax)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Fax),
                                      new LimitedLengthPropertyFeature(50)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.WorkEmail)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WorkEmail),
                                      new LimitedLengthPropertyFeature(100)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.HomeEmail)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.HomeEmail),
                                      new LimitedLengthPropertyFeature(100)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Website)
                                  .WithFeatures(
                                      new UrlPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WebsiteOrBlog),
                                      new LimitedLengthPropertyFeature(200)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ImIdentifier)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Im),
                                      new LimitedLengthPropertyFeature(64)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.JobTitle)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.JobTitle),
                                      new LimitedLengthPropertyFeature(170)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Department)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ContactDepartmentName),
                                      new LimitedLengthPropertyFeature(100)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.WorkAddress)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WorkAddress),
                                      new LimitedLengthPropertyFeature(512)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.HomeAddress)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.HomeAddress),
                                      new LimitedLengthPropertyFeature(512)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(3),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment),
                                      new LimitedLengthPropertyFeature(512)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.GenderCode)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Gender)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.FamilyStatusCode)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FamilyStatus)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.AccountRole)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AccountRole)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Client()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Client)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ClientReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.IsFired)
                                  .WithFeatures(new YesNoRadioPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.IsFired)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.BirthDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BirthDate)),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ContactDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
