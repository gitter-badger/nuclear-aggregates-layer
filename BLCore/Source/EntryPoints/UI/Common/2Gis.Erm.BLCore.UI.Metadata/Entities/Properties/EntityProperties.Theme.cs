using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> ThemeProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new NonZeroPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Id)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.ThemeTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.ThemeTemplate()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeTemplate)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.BeginDistribution)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.WithoutDayNumber),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginDistributionDate)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.EndDistribution)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyUpperBound, DisplayStyle.WithoutDayNumber),
                                      MustBeGreaterOrEqualPropertyFeature.Create<ThemeDomainEntityDto>(dto => dto.BeginDistribution,
                                                                                                       () =>
                                                                                                       BLResources
                                                                                                           .EndDistributionDateMustBeGreaterOrEqualThanBeginDistributionDate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndDistributionDate)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.IsDefault)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      new HiddenFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeIsDefault)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeViewModel_FileId)),

                   EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),


                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ThemeDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            })
                };
    }
}
