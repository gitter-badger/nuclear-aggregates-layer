using DoubleGis.Erm.API.WCF.Metadata.DI;
using DoubleGis.Erm.API.WCF.Metadata.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.API.WCF.Metadata
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<MetadataServiceAppSettings>
    {
        public UnityServiceHostFactory()
            : base(new MetadataServiceAppSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}