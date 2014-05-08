namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration
{
    /// <summary>
    /// Интерфейс для служебных (системных) сущностей, которые необходимы для хранения состояния обработчиков интеграции
    /// Пока не реализован транспорт для событий о выполняемых операциях в системе (например, servicebus) с помощью DIFF между 
    /// набором данного типа сущностей и perfomedbusinessoperations можно получить набор событий системы на которые требуется реакция
    /// обработчика интеграции
    /// </summary>
    public interface IIntegrationProcessorState : IEntity, IEntityKey
    {
        System.DateTime Date { get; set; }
    }
}