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
        public static readonly IEnumerable<EntityProperty> AssociatedPositionProperties =
            new[]
                {
                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.AssociatedPositionsGroupRef)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AssociatedPositionsGroupName)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.PositionRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    new EntityProperty("PositionName", typeof(string)) 
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.PricePositionRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PricePosition)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.ObjectBindingType)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ObjectBindingType)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.PriceIsDeleted),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.PriceIsPublished),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new HiddenFeature()),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<AssociatedPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
