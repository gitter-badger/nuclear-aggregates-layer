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
        public static readonly IEnumerable<EntityPropertyMetadata> FirmAddressProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.FirmRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Firm),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Firm)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.Address)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Address)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.PaymentMethods)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PaymentMethods)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.WorkingTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.WorkingTime)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.ClosedForAscertainment)
                                  .WithFeatures(
                                      new YesNoRadioPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsHidden)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<FirmAddressDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
