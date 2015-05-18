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
        public static readonly IEnumerable<EntityPropertyMetadata> PositionProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.IsPublished)
                    .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.IsComposite)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CompositePosition)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.CalculationMethodEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CalculationMethod)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.PositionsGroup)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PositionsGroup)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.SalesModel)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SalesModel)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.PlatformRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Platform()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.CategoryRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.PositionCategory()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.AdvertisementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.AdvertisementTemplate()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.ExportCode)
                                  .WithFeatures(
                                  new RequiredPropertyFeature(),
                                  RangePropertyFeature.Create(0, int.MaxValue, () => BLResources.ExportCodeRangeMessage),
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.ExportCode)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.DgppId)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.DgppId)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.IsControlledByAmount)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.IsControledByAmount)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.BindingObjectTypeEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BindingObjectType)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
