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
        public static readonly IEnumerable<EntityProperty> DeniedPositionProperties =
            new[]
                {
                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.PriceRef)
                                  .WithFeatures(new PresentationLayerPropertyFeature(),
                                                new HiddenFeature()),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.PositionRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new ReadOnlyPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityName.Position)
                                                                     .WithSearchFormFilterInfo("IsDeleted=false"),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.PositionDeniedRef)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                LookupPropertyFeature.Create(EntityName.Position)
                                                                     .WithSearchFormFilterInfo("IsDeleted=false"),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.ObjectBindingType)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new EnumPropertyFeature(EnumResources.ResourceManager),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.ObjectBindingType)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.IsPricePublished),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),


                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<DeniedPositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
