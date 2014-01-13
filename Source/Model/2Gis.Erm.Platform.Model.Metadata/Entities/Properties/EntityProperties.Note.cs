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
        public static readonly IEnumerable<EntityProperty> NoteProperties =
            new[]
                {
                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.Title)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Title)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.Text)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Text)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.FileId)
                                  .WithFeatures(
                                      new FilePropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.FileId)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.FileName)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.FileContentType)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.FileContentLength)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),
                 
                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.ParentRef)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.ParentTypeName)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<NoteDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
