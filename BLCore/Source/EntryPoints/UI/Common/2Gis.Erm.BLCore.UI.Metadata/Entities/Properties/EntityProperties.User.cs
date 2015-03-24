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
        public static readonly IEnumerable<EntityPropertyMetadata> UserProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.FirstName)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.FirstName)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.LastName)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.LastName)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.Account)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.UserAccount)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.DepartmentRef)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    LookupPropertyFeature.Create(EntityType.Instance.Department())),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.ParentRef)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.ParentUser)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.IsServiceUser)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.IsServiceUser)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.DisplayName),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<UserDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
