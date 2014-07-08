using System;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Flows
{
    /// <summary>
    /// Абстракция для описателя потока сообщений в системе, основное назначение - уникальная идентификация потоков данных
    /// </summary>
    public interface IMessageFlow : IEquatable<IMessageFlow>
    {
        /// <summary>
        /// Уникальный идентификатор операции - уникально идентифицирует сообщение
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Описание сообщения - назначение, в чем суть и т.п.
        /// </summary>
        string Description { get; }
    }
}