using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> DealProperties =
            new[]
                {
                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.Name)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(300),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DealName)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.DealStage)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.DealStage)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.EstimatedProfit)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.EstimatedProfit)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ActualProfit)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ActualProfit)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.Comment)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new MultilinePropertyFeature(5),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Comment)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CloseDate)
                                  .WithFeatures(
                                      new DatePropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseDate)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.StartReason)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.StartReason)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CloseReason)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new EnumPropertyFeature(EnumResources.ResourceManager),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseReason)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CloseReasonOther)
                                  .WithFeatures(
                                      new LimitedLengthPropertyFeature(512),
                                      new ReadOnlyPropertyFeature(),
                                      new MultilinePropertyFeature(3),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CloseReasonOther)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CurrencyRef)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Currency),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Currency)),

                    new EntityProperty("IsSecurityRoot", typeof(bool))
                        .WithFeatures(
                            new OnlyValuePropertyFeature<bool>(true),
                            new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ClientRef)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      LookupPropertyFeature.Create(EntityName.Client),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Client)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ClientReplicationCode)
                                  .WithFeatures(new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.MainFirmRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.Firm)
                                                           .WithReadOnly()
                                                           .WithExtendedInfo("'clientId={ClientRef.Id}'"),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.MainFirm)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CreatedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedBy)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.CreatedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ModifiedByRef)
                                  .WithFeatures(
                                      LookupPropertyFeature.Create(EntityName.User),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ModifiedBy)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.ModifiedOn)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.CreatedOn)),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.Timestamp)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.IsActive)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature()),

                    EntityProperty.Create<DealDomainEntityDto>(dto => dto.IsDeleted)
                                  .WithFeatures(
                                      new RequiredPropertyFeature(),
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
