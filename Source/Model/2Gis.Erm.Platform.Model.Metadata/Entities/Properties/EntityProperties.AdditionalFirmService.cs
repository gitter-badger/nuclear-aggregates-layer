using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        public static readonly IEnumerable<EntityProperty> AdditionalFirmServiceProperties =
            new[]
                {
                    EntityProperty.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.ServiceCode)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.ServiceCode)),

                    EntityProperty.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.Description)
                                  .WithFeatures(
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.Description)),
                    EntityProperty.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.IsManaged)
                                  .WithFeatures(
                                      new YesNoRadioPropertyFeature(),
                                      DisplayNameLocalizedFeature.Create(() => MetadataResources.IsManaged)),

                    EntityProperty.Create<AdditionalFirmServiceDomainEntityDto>(dto => dto.Id)
                                  .WithFeatures(
                                      new ReadOnlyPropertyFeature(),
                                      new HiddenFeature())
                };
    }
}
