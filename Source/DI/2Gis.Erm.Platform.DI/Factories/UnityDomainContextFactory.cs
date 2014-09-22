using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
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

        public UnityDomainContextFactory(IEFConnectionFactory connectionFactory,
                                         IDomainContextMetadataProvider domainContextMetadataProvider,
                                         IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                                         IProducedQueryLogAccessor producedQueryLogAccessor,
                                         ICommonLog logger,
                                         IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider,
                                         IUnityContainer unityContainer)
            : base(
                connectionFactory,
                domainContextMetadataProvider,
                pendingChangesHandlingStrategy,
                producedQueryLogAccessor,
                logger,
                msCrmReplicationMetadataProvider)
        {
            _unityContainer = unityContainer;
        }

        protected override IProcessingContext ProcessingContext
        {
            get
            {
                return _unityContainer.Resolve<IProcessingContext>();
            }
        }
    }
}
