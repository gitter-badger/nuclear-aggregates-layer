using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List;
using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations;
using DoubleGis.Erm.Qds.Operations.Indexers;
using DoubleGis.Erm.Qds.Operations.Indexers.Raw;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static partial class QueryingBootstrapper
    {
        public static IUnityContainer ConfigureListing(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            container.ConfigureQdsListing(entryPointSpecificLifetimeManagerFactory);
            container.ConfigureQdsIndexing(entryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer ConfigureQdsListing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            // FIXME {all, 02.07.2014}: убрать отсюда регистрацию settings - это происходит при обработке settingscontainer
            var connectionStringsAspect = new ConnectionStringsSettingsAspect();
            var searchConnectionString = connectionStringsAspect.GetConnectionString(ConnectionStringName.ErmSearch);
            var searchSettings = new NestSettingsAspect(searchConnectionString);

            // TODO: заменить на правильное
            container
                .RegisterType<IUserProfile, NullUserProfile>(Lifetime.Singleton);

            container
                .RegisterType<IElasticResponseHandler, ElasticResponseHandler>(Lifetime.Singleton)
                .RegisterInstance<INestSettings>(searchSettings, Lifetime.Singleton)
                .RegisterType<UnityElasticApiFactory>(Lifetime.Singleton)
                .RegisterType<IElasticApi>(lifetime(), new InjectionFactory(x => container.Resolve<UnityElasticApiFactory>().CreateElasticApi(lifetime)))
                .RegisterType<IElasticManagementApi>(lifetime(), new InjectionFactory(x => (IElasticManagementApi)container.Resolve<IElasticApi>()));

            return container;
        }

        private static IUnityContainer ConfigureQdsIndexing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            container
                .RegisterType<IEntityIndexer<User>, UserIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<User>, UserIndexer>(lifetime())

                .RegisterType<IEntityIndexer<Territory>, TerritoryIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<Territory>, TerritoryIndexer>(lifetime())

                .RegisterType<IEntityIndexer<Client>, ClientIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<Client>, ClientIndexer>(lifetime())

                .RegisterType<IEntityIndexer<Order>, OrderIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<Order>, OrderIndexer>(lifetime())

                .RegisterType<IEntityIndexer<Firm>, FirmIndexer>(lifetime())
                .RegisterType<IEntityIndexerIndirect<Firm>, FirmIndexer>(lifetime())

                .RegisterType<IDocumentIndexer<UserDoc>, UserIndexer>(lifetime())
                .RegisterType<IDocumentIndexer<TerritoryDoc>, TerritoryIndexer>(lifetime())
                .RegisterType<IDocumentIndexer<ClientGridDoc>, ClientIndexer>(lifetime())
                .RegisterType<IDocumentIndexer<OrderGridDoc>, OrderIndexer>(lifetime())
                .RegisterType<IDocumentIndexer<FirmGridDoc>, FirmIndexer>(lifetime())
                .RegisterType<IDefferedDocumentUpdater, DefferedDocumentUpdater>(lifetime())

                .RegisterType<IRawEntityIndexer, RawEntityIndexer>(lifetime());

            return container;
        }

        public static Type ListServiceConflictResolver(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            if (entitySet.Entities.Contains(EntityName.Order))
            {
                var businessModel = ConfigFileSetting.Enum.Required<BusinessModel>("BusinessModel").Value;
                if (businessModel == BusinessModel.Russia)
                {
                    return candidates.Single(x => x.Assembly == typeof(QdsListOrderService).Assembly);
                }

                return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
            }

            return candidates.Single(x => x.Assembly == typeof(ListClientService).Assembly);
        }
    }
}
