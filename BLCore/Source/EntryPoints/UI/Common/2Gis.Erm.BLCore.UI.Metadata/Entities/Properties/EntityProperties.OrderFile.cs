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
        public static readonly IEnumerable<EntityPropertyMetadata> OrderFileProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.Id),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.OrderId)
                                  .WithFeatures(new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.FileKind)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileKind)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new FilePropertyFeature(),
                                      new RequiredPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<OrderFileDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
