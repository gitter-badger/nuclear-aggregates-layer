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
        public static readonly IEnumerable<EntityProperty> ClientProperties =
            new[]
                {
                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(
                                      new HiddenFeature(),
                                      new PresentationLayerPropertyFeature()),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new LimitedLengthPropertyFeature(250),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.NameOfClient)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.MainAddress)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainAddress)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.MainPhoneNumber)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainPhoneNumber)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.AdditionalPhoneNumber1)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalPhoneNumber1)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.AdditionalPhoneNumber2)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(64),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.AdditionalPhoneNumber2)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Email)
                                  .WithFeatures(
                                      new EmailPropertyFeature(),
                                      new LimitedLengthPropertyFeature(100),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Email)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Fax)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(50),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Fax)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Website)
                                  .WithFeatures(
                                      new UrlPropertyFeature(),
                                      new LimitedLengthPropertyFeature(200),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Website)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.MainFirmRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.Firm)
                                                           .WithSearchFormFilterInfo("ClientId={Id}"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainFirm)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.TerritoryRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Territory),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Territory)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.InformationSource)
                                  .WithFeatures(new RequiredPropertyFeature(),
                                                new EnumPropertyFeature(EnumResources.ResourceManager),
                                                DisplayNameLocalizedFeature.Create(() => MetadataResources.InformationSource)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.PromisingValue)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.PromisingScore)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.LastQualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastQualifyTime)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.LastDisqualifyTime)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new DatePropertyFeature(false),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.LastDisqualifyTime)),

                    new EntityProperty("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<ClientDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
