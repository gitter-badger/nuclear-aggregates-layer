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
        public static readonly IEnumerable<EntityProperty> LocalMessageProperties =
            new[]
                {
                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.EventDate)
                                  .WithFeatures(DisplayNameLocalizedFeature.Create(() => MetadataResources.ProcessDate)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.SenderSystem)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.SenderSystem)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.ReceiverSystem)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ReceiverSystem)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.Status)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Status)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.IntegrationTypeImport)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IntegrationType)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.IntegrationTypeExport)
                                  .WithFeatures(
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IntegrationType)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.OrganizationUnitRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.OrganizationUnit),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.OrganizationUnit)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.ProcessResult)
                                  .WithFeatures(
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ProcessResult)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<LocalMessageDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
