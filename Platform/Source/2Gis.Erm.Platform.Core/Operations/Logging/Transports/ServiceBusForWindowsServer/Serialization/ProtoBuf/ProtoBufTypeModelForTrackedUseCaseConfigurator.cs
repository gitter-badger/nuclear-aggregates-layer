using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

using ProtoBuf.Meta;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf
{
    public static class ProtoBufTypeModelForTrackedUseCaseConfigurator
    {
        public static RuntimeTypeModel Configure()
        {
            var typeModel = TypeModel.Create();
            typeModel.Add(typeof(TrackedUseCase), false)
                     .Add(1, "_rootNodeId")
                     .Add(2, "_operations")
                     .Add(3, "_operationsHierarchy");
            typeModel.Add(typeof(OperationScopeNode), false)
                     .Add(1, "_operationIdentity")
                     .Add(2, "_isRoot")
                     .Add(3, "_scopeId")
                     .Add(4, "_changesContext")
                     .UseConstructor = false;
            typeModel.Add(typeof(IOperationScope), false)
                     .AddSubType(1, typeof(TransactedOperationScope));
            typeModel.Add(typeof(EntityChangesContext), false)
                     .Add(1, "_addedStorage")
                     .Add(2, "_updatedStorage")
                     .Add(3, "_deletedStorage");
            typeModel.Add(typeof(TransactedOperationScope), false)
                     .Add(1, "_scopeId")
                     .Add(2, "_isRootScope")
                     .Add(3, "_strictOperationIdentity")
                     .UseConstructor = false;
            typeModel.Add(typeof(StrictOperationIdentity), false)
                     .Add(1, "_operationIdentity")
                     .Add(2, "_entitySet")
                     .UseConstructor = false;
            typeModel.Add(typeof(EntitySet), false)
                     .Add(1, "_entities")
                     .UseConstructor = false;
            typeModel.Add(typeof(IOperationIdentity), false)
                     .SetSurrogate(typeof(OperationIdentitySurrogate));

            return typeModel;
        }
    }
}