using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    public interface IDeserializeServiceBusDtoServiceFactory
    {
        bool TryGetDeserializeServiceBusObjectServices(string flowName,
                                                       string busObjectTypeName,
                                                       out IReadOnlyCollection<IDeserializeServiceBusObjectService> services);
    }
}