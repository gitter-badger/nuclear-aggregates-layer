using System;

using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Documents;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Operations.Indexers;
using DoubleGis.Erm.Qds.Operations.Indexers.Raw;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.Migrator.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, SearchSettings searchSettings)
        {
            var fakeAppSettings = new FakeAppSettings(searchSettings);

            container
                .RegisterInstance<IAppSettings>(fakeAppSettings, Lifetime.Singleton)
                .RegisterInstance<IMsCrmSettings>(fakeAppSettings, Lifetime.Singleton)
                .ConfigureDAL(() => Lifetime.Singleton, fakeAppSettings);

            container
                .ConfigureSearch(searchSettings, () => Lifetime.Singleton);

            return container;
        }

        private static IUnityContainer ConfigureSearch(this IUnityContainer container, SearchSettings searchSettings, Func<LifetimeManager> lifetime)
        {
            // TODO: заменить на правильное
            container
                .RegisterType<IUserProfile, NullUserProfile>(Lifetime.Singleton);

            container
                .RegisterType<IEntityIndexer<User>, UserIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<User>, UserIndexer>(lifetime())
                .RegisterType<IEntityIndexer<Client>, ClientIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<Client>, ClientIndexer>(lifetime())

                .RegisterType<IDocumentIndexer<UserDoc>, UserIndexer>(lifetime())
                .RegisterType<IDocumentIndexer<ClientGridDoc>, ClientIndexer>(lifetime())
                .RegisterType<IRawDocumentIndexer, RawDocumentIndexer>(lifetime())
                .RegisterType<IRawEntityIndexer, RawEntityIndexer>(lifetime());

            container
                .RegisterInstance<ISearchSettings>(searchSettings, Lifetime.Singleton)
                .RegisterInstance<IEnvironmentIsolationSettings>(searchSettings, Lifetime.Singleton);

            container
                .RegisterType<IElasticClientFactory, ElasticClientFactory>(Lifetime.Singleton)
                .RegisterType<IElasticConnectionSettingsFactory, ElasticConnectionSettingsFactory>(Lifetime.Singleton)
                .RegisterType<IElasticApi, ElasticApi>(Lifetime.Singleton);

            return container;
        }
    }
}