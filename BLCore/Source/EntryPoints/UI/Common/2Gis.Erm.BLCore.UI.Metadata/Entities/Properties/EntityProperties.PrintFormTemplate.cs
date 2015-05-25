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
        public static readonly IEnumerable<EntityPropertyMetadata> PrintFormTemplateProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.TemplateCode)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.TemplateCode)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                  new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.BranchOfficeOrganizationUnitRef)
                                  .WithFeatures(
                                      new PresentationLayerPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<PrintFormTemplateDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
