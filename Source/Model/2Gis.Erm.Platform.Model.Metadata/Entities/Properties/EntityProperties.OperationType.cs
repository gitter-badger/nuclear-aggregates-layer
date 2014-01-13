using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> OperationTypeProperties =
            new[]
                {
                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.Id),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationTypeDescription)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.IsPlus)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OperationTypeKind)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.IsInSyncWith1C)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.InSyncWith1C)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.SyncCode1C)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SyncCode1C)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.IdentityServiceUrl)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<OperationTypeDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
