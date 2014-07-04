namespace DoubleGis.Erm.Platform.API.Core.Messaging.Flows
{
    /// <summary>
    /// Абстракция позволяющая указать для конкретного типа-маркера реализующего IMessageFlow, message flow источник (из которого данные попадают в данный поток)
    /// </summary>
    public interface ISourceMessageFlow<TMessageFlow>
        where TMessageFlow : class, IMessageFlow
    {
    }
}