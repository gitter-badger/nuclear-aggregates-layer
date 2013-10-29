using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public interface ISharedTypesBehaviorFactory
    {
        IEndpointBehavior Create();
    }
}