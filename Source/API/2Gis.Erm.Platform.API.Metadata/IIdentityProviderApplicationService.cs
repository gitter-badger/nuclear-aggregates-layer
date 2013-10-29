using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.Platform.API.Metadata
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.Identity.Identity201303)]
    public interface IIdentityProviderApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Identity.Identity201303)]
        long[] GetIdentities(int count);
    }
}
