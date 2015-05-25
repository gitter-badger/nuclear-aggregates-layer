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
        public static readonly IEnumerable<EntityPropertyMetadata> ClientProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(250),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.NameOfClient)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.MainAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainAddress)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.MainPhoneNumber)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainPhoneNumber)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.AdditionalPhoneNumber1)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalPhoneNumber1)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.AdditionalPhoneNumber2)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalPhoneNumber2)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Email)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(100),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Email)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Fax)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(50),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Fax)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Website)
                                  .WithFeatures(
                                      new UrlPropertyFeature(),
                                      new LimitedLengthPropertyFeature(200),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Website)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.MainFirmRef)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainFirm)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.TerritoryRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Territory()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Territory)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.InformationSource)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new EnumPropertyFeature(EnumResources.ResourceManager),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.InformationSource)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.PromisingValue)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PromisingScore)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.LastQualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastQualifyTime)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.LastDisqualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastDisqualifyTime)),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<ClientDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
