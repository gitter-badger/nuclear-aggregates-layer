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
        public static readonly IEnumerable<EntityPropertyMetadata> AssociatedPositionProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.AssociatedPositionsGroupRef)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AssociatedPositionsGroupName)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.PositionRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    new EntityPropertyMetadata("PositionName", typeof(string)) 
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.PricePositionRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePosition)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.ObjectBindingType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ObjectBindingType)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.PriceIsDeleted),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.PriceIsPublished),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<AssociatedPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
