using System.ServiceModel;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors.Lifetime
{
    /// <summary>
    /// Implements the lifetime manager storage for the <see cref="System.ServiceModel.OperationContext"/> extension.
    /// </summary>
    public class LifetimeManagementOperationContextExtension : WcfKeyValueExtension<OperationContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifetimeManagementOperationContextExtension"/> class.
        /// </summary>
        public LifetimeManagementOperationContextExtension() : base()
        {
        }

        /// <summary>
        /// Gets the <see cref="LifetimeManagementOperationContextExtension"/> for the current operation context.
        /// </summary>
        public static LifetimeManagementOperationContextExtension Current
        {
            get { return OperationContext.Current.Extensions.Find<LifetimeManagementOperationContextExtension>(); }
        }
    }
}
