﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> LockDetailProperties =
            new[]
                {
                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LockDetailCreateDate)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.Amount)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Amount)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.PriceRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Price),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PriceList)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(200),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.LockRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LockDetailDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
