using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> ThemeProperties =
            new[]
                {
                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new DigitsOnlyPropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      new NonZeroPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Id)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Name)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.ThemeTemplateRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.ThemeTemplate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeTemplate)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.BeginDistribution)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyLowerBound, DisplayStyle.WithoutDayNumber),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.BeginDistributionDate)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.EndDistribution)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new DatePropertyFeature(false, PeriodType.MonthlyUpperBound, DisplayStyle.WithoutDayNumber),
                                      MustBeGreaterOrEqualPropertyFeature.Create<ThemeDomainEntityDto>(dto => dto.BeginDistribution,
                                                                                                       () =>
                                                                                                       BLResources
                                                                                                           .EndDistributionDateMustBeGreaterOrEqualThanBeginDistributionDate),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EndDistributionDate)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.IsDefault)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new YesNoRadioPropertyFeature(),
                                      new HiddenFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeIsDefault)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ThemeViewModel_FileId)),

                   EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),


                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.IdentityServiceUrl)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),


                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<ThemeDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    new EntityProperty
                        {
                            Name = "IsSecurityRoot",
                            Type = typeof(bool),
                        }
                        .WithFeatures(new IPropertyFeature[]
                            {
                                new OnlyValuePropertyFeature<bool>(true),
                                new HiddenFeature()
                            })
                };
    }
}
