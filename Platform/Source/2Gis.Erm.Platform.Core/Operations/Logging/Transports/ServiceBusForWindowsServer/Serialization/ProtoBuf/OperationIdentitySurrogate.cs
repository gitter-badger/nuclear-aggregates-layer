using System;

using DoubleGis.Erm.Platform.Common.Utils;
using NuClear.Model.Common.Operations.Identity;

using ProtoBuf;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf
{
    [ProtoContract]
    public sealed class OperationIdentitySurrogate
    {
        [ProtoMember(1)]
        public string AssemblyQualifiedName { get; set; }

        [ProtoConverter]
        public static IOperationIdentity From(OperationIdentitySurrogate value)
        {
            return value == null ? null : (IOperationIdentity)Type.GetType(value.AssemblyQualifiedName).New();
        }

        [ProtoConverter]
        public static OperationIdentitySurrogate To(IOperationIdentity value)
        {
            return value == null ? null : new OperationIdentitySurrogate { AssemblyQualifiedName = value.GetType().AssemblyQualifiedName };
        }
    }
}