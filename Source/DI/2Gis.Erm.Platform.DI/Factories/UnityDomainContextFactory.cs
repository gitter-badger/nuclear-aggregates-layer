using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    public sealed class UnityDomainContextFactory : EFDomainContextFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityDomainContextFactory(
            IUnityContainer unityContainer, 
            IEFConnectionFactory connectionFactory, 
            IDomainContextMetadataProvider domainContextMetadataProvider, 
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy, 
            IMsCrmSettings msCrmSettings, 
            ICommonLog logger)
            : base(connectionFactory, domainContextMetadataProvider, pendingChangesHandlingStrategy, msCrmSettings, logger)
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
