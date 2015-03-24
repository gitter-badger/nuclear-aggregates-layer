using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityPropertyMetadata> AdditionalFirmServiceProperties =
            new[]
                {
                    EntityPropertyMetadata.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.ServiceCode)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ServiceCode)),

                    EntityPropertyMetadata.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description)),
                    EntityPropertyMetadata.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.IsManaged)
                                  .WithFeatures(
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsManaged)),

                    EntityPropertyMetadata.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
