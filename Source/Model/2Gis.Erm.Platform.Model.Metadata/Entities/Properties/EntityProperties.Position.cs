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
        public static readonly IEnumerable<EntityProperty> PositionProperties =
            new[]
                {
                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.Id),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.IsPublished)
                    .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(256),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Position)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.IsComposite)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CompositePosition)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.CalculationMethodEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CalculationMethod)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.AccountingMethodEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AccountingMethod)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.PlatformRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Platform),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.CategoryRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.PositionCategory),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.AdvertisementTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.AdvertisementTemplate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Platform)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.ExportCode)
                                  .WithFeatures(
                                  new RequiredPropertyFeature(),
                                  RangePropertyFeature.Create(0, int.MaxValue, () => BLResources.ExportCodeRangeMessage),
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.ExportCode)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.DgppId)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.DgppId)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.IsControlledByAmount)
                                  .WithFeatures(
                                  DisplayNameLocalizedFeature.Create(() => MetadataResources.IsControledByAmount)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.BindingObjectTypeEnum)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BindingObjectType)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<PositionDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
