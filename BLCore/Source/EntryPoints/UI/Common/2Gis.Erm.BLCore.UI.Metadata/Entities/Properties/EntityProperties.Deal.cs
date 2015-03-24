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
        public static readonly IEnumerable<EntityPropertyMetadata> DealProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DealName)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.DealStage)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DealStage)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseDate)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.StartReason)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.StartReason)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CloseReason)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseReason)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CloseReasonOther)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new ReadOnlyPropertyFeature(),
                                      new MultilinePropertyFeature(3),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseReasonOther)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Currency()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Currency)),

                    new EntityPropertyMetadata("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityType.Instance.Client()),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Client)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.ClientReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.MainFirmRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.Firm())
                                                           .WithReadOnly()
                                                           .WithExtendedInfo("'clientId={ClientRef.Id}'"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainFirm)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityType.Instance.User()),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityPropertyMetadata.Create<DealDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
