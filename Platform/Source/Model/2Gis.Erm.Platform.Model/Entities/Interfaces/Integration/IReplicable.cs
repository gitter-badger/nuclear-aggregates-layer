using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration
{
    public interface IReplicableEntity : IEntityKey
    {
        Guid ReplicationCode { get; set; }
    }
}