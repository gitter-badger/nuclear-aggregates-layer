using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations;
using DoubleGis.Erm.Qds.Operations.Indexers;
using DoubleGis.Erm.Qds.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static partial class QueryingBootstrapper
    {
        public static IUnityContainer ConfigureListing(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            container
                .RegisterType<IQuerySettingsProvider, QuerySettingsProvider>(entryPointSpecificLifetimeManagerFactory());

            //container.RegisterType<IQuerySettingsProvider>(
            //    entryPointSpecificLifetimeManagerFactory(),
            //    new InjectionFactory(x =>
            //                         new QuerySettingsProviderSelector(
            //                             x.Resolve<QuerySettingsProvider>(),
            //                             x.Resolve<QdsQuerySettingsProvider>())));
            // container.ConfigureQdsListing(entryPointSpecificLifetimeManagerFactory);

            return container;
        }

        //private static IUnityContainer ConfigureQdsListing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        //{
        //    // TODO: заменить на правильное
        //    container
        //        .RegisterType<IUserProfile, NullUserProfile>(Lifetime.Singleton);

        //    container
        //        .RegisterType<IEntityIndexer<User>, UserIndexer>(lifetime())
        //        .RegisterType<IEntityIndexerIndirect<User>, UserIndexer>(lifetime())
        //        .RegisterType<IEntityIndexer<Client>, ClientIndexer>(lifetime())
        //        .RegisterType<IEntityIndexerIndirect<Client>, ClientIndexer>(lifetime())

        //        .RegisterType<IDocumentIndexer<UserDoc>, UserIndexer>(lifetime())
        //        .RegisterType<IDocumentIndexer<ClientGridDoc>, ClientIndexer>(lifetime())
        //        .RegisterType<IRawDocumentIndexer, RawDocumentIndexer>(lifetime())
        //        .RegisterType<IRawEntityIndexer, RawEntityIndexer>(lifetime());

        //    container
        //        .RegisterInstance<ISearchSettings>(new SearchSettings(), Lifetime.Singleton);

        //    container
        //        .RegisterType<IElasticClientFactory, ElasticClientFactory>(Lifetime.Singleton)
        //        .RegisterType<IElasticConnectionSettingsFactory, ElasticConnectionSettingsFactory>(Lifetime.Singleton)
        //        .RegisterType<IElasticApi, ElasticApi>(Lifetime.Singleton);

        //    return container;
        //}

        public static Type ListServiceConflictResolver(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            //if (entitySet.Entities.Contains(EntityName.Client))
            //{
            //    return candidates.Single(x => x.Assembly == typeof(QdsListClientService).Assembly);
            //}

            return candidates.Single(x => x.Assembly == typeof(ListClientService).Assembly);
        }

    }
}
