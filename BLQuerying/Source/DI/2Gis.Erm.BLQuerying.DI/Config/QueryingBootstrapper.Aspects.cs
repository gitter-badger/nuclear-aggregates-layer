using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Settings;

using Microsoft.Practices.Unity;

using Nest;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static class QueryingBootstrapper
    {
        public static IUnityContainer ConfigureElasticApi(this IUnityContainer container, INestSettings nestSettings)
        {
            return container
                .RegisterInstance(nestSettings)

                .RegisterType<IConnectionSettingsValues, CommonConnectionSettings>(Lifetime.Singleton, new InjectionConstructor(typeof(INestSettings), IndexMappingMetadata.DocTypeToIndexNameMap))
                .RegisterType<IElasticMetadataApi, CommonConnectionSettings>()

                .RegisterType<IElasticApi, ElasticApi>(Lifetime.Singleton)
                .RegisterType<IElasticManagementApi, ElasticApi>(Lifetime.Singleton);
        }
    }
}