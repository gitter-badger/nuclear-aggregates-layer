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
        public static readonly IEnumerable<EntityPropertyMetadata> BranchOfficeProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.DgppId)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DgppId)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.LegalAddress)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LegalAddress)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.Inn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(10, 12),
                                      new DigitsOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Inn)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.BargainTypeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.BargainType())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainType)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.ContributionTypeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.ContributionType())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ContributionType)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.Annotation)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(10),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Annotation)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.UsnNotificationText)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(10),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.UsnNotificationText)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BranchOfficeDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
