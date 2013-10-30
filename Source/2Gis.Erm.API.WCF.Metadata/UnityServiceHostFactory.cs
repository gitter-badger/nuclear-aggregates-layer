using DoubleGis.Erm.API.WCF.Metadata.DI;
using DoubleGis.Erm.API.WCF.Metadata.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.WCF.Metadata.Settings;

namespace DoubleGis.Erm.API.WCF.Metadata
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IMetadataServiceAppSettings, MetadataServiceAppSettings>
    {
        public UnityServiceHostFactory()
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}
