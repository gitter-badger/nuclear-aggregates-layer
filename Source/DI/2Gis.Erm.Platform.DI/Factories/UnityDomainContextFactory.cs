using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    public sealed class UnityDomainContextFactory : EFDomainContextFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityDomainContextFactory(IDomainContextMetadataProvider domainContextMetadataProvider,
                                         IConnectionStringSettings connectionStringSettings,
                                         IEfDbModelFactory efDbModelFactory,
                                         IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                                         IProducedQueryLogAccessor producedQueryLogAccessor,
                                         ICommonLog logger,
                                         IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider,
                                         IUnityContainer unityContainer)
            : base(
                domainContextMetadataProvider,
                connectionStringSettings,
                efDbModelFactory,
                pendingChangesHandlingStrategy,
                producedQueryLogAccessor,
                logger,
                msCrmReplicationMetadataProvider)
        {
            _unityContainer = unityContainer;
        }

        protected override IProcessingContext ProcessingContext
        {
            get { return _unityContainer.Resolve<IProcessingContext>(); }
        }
    }
}