using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// Маркерный интерфейс для всех высокоуровневых операций Erm выполняемых, специфичным образом над некоторым набором сущностей ERM  - все операции формируют множество applicationservices,
    /// используя которые можно обрабатывать агрегаты системы
    /// Т.о. конкретная реализация операции может зависить/определяться не от одного типа сущности, а сразу от нескольких, например операция append.
    /// Примеры специфичных операций: 
    ///  - activate(client)
    ///  - append(Role->User)
    /// </summary>
    public interface IEntityOperation : IOperation
    {
    }

    /// <summary>
    /// Маркерный интерфейс для операций, конкретная реализация которых специфична для одного конкретного типа сущности
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности для которой реализуется операция</typeparam>
    public interface IEntityOperation<TEntity> : IEntityOperation
        where TEntity : IEntityKey
    {
    }

    /// <summary>
    /// Маркерный интерфейс для операций, конкретная реализация которых специфична для двух конкретных типов сущностей
    /// </summary>
    /// <typeparam name="TEntity1">Тип сущности 1ой, специфично для которой реализуется операция</typeparam>
    /// <typeparam name="TEntity2">Тип сущности 2ой, специфично для которой реализуется операция</typeparam>
    public interface IEntityOperation<TEntity1, TEntity2> : IEntityOperation
        where TEntity1 : IEntityKey
        where TEntity2 : IEntityKey
    {
    }
}