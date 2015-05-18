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
        public static readonly IEnumerable<EntityPropertyMetadata> BargainProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.Number)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainNumber)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.BargainTypeRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.BargainType())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainType)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.CustomerLegalPersonRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.LegalPerson())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BargainLegalPerson)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.ExecutorBranchOfficeRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.BranchOfficeOrganizationUnit())
                                                           .WithShowReadOnlyCard(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BranchOfficeOrganizationUnit)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                  new MultilinePropertyFeature(4),
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.SignedOn)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SignedOn)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.ClosedOn)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ClosedOn)),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.HasDocumentsDebt)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.HasDocumentsDebt)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.DocumentsComment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DocumentsComment)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<BargainDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
