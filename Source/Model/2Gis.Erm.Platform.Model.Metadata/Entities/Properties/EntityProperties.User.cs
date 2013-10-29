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
        public static readonly IEnumerable<EntityProperty> UserProperties =
            new[]
                {
                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.Id),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.FirstName)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.FirstName)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.LastName)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.LastName)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.Account)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.UserAccount)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.DepartmentRef)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    LookupPropertyFeature.Create(EntityName.Department)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.ParentRef)
                    .WithFeatures(
                    new RequiredPropertyFeature(),
                    DisplayNameLocalizedFeature.Create(() => MetadataResources.ParentUser),
                    LookupPropertyFeature.Create(EntityName.User)
                                         .WithSearchFormFilterInfo("Id!={Id}")),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.IsServiceUser)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.IsServiceUser)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.DisplayName),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<UserDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
