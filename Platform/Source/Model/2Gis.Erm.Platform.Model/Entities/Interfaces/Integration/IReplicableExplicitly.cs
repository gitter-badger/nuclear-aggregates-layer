namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration
{
    /// <summary>
    /// for entities, that have no replication code, but should be replicated
    /// </summary>
    public interface IReplicableExplicitly : IEntityKey
    {
    }
}