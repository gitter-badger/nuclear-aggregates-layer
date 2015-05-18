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
        public static readonly IEnumerable<EntityPropertyMetadata> NoteProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.Title)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Title)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.Text)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),
                 
                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.ParentRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.ParentTypeName)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<NoteDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
