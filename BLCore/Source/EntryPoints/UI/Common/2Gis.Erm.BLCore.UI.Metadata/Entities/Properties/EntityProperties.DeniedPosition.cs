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
        public static readonly IEnumerable<EntityPropertyMetadata> DeniedPositionProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.PriceRef)
                                  .WithFeatures(new PresentationLayerPropertyFeature(),
                                                new HiddenFeature()),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.PositionRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new ReadOnlyPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.PositionDeniedRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.ObjectBindingType)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new EnumPropertyFeature(EnumResources.ResourceManager),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ObjectBindingType)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.PriceIsPublished),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<DeniedPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
