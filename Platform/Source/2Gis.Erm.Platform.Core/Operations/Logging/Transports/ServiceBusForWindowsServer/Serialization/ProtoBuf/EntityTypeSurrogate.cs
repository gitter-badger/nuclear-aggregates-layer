using NuClear.Model.Common.Entities;

using ProtoBuf;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf
{
    [ProtoContract]
    public sealed class EntityTypeSurrogate
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoConverter]
        public static IEntityType From(EntityTypeSurrogate value)
        {
            return value == null ? null : EntityType.Instance.Parse(value.Id);
        }

        [ProtoConverter]
        public static EntityTypeSurrogate To(IEntityType value)
        {
            return value == null ? null : new EntityTypeSurrogate { Id = value.Id };
        }
    }
}