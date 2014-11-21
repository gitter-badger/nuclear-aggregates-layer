using System.ServiceModel;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors.Lifetime;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    /// <summary>
    /// Unity lifetime manager to support <see cref="System.ServiceModel.OperationContext"/>.
    /// </summary>
    public class UnityOperationContextLifetimeManager : UnityWcfLifetimeManager<OperationContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityOperationContextLifetimeManager"/> class.
        /// </summary>
        public UnityOperationContextLifetimeManager() : base()
        {
        }

        /// <summary>
        /// Returns the appropriate extension for the current lifetime manager.
        /// </summary>
        /// <returns>The registered extension for the current lifetime manager, otherwise, null if the extension is not registered.</returns>
        protected override WcfKeyValueExtension<OperationContext> FindExtension()
        {
            return LifetimeManagementOperationContextExtension.Current;
        }
    }
}
